using Newtonsoft.Json;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileParser : AbstractBilingualParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IFileHelper _fielHelper;
        private readonly IJsonFactory _jsonFactory;
        private readonly IEmbeddedContentRegexSettings _embeddedContentRegexSettings;
        private readonly IParsingSettings _parsingSettings;
        private readonly IParagraphUnitFactory _paragraphUnitFactory;
        private readonly ISegmentDataCollector _segmentDataCollector;

        //State during parsing
        private IPersistentFileConversionProperties _fileConversionProperties;
        private IJsonTextReader _reader;
        private int _totalNumberOfLines;
        private byte _progressInPercent;

        public JsonFileParser(IJsonFactory jsonFactory, IFileHelper fielHelper, IEmbeddedContentRegexSettings embeddedContentRegexSettings, IParsingSettings parsingSettings, IParagraphUnitFactory paragraphUnitFactory, ISegmentDataCollector segmentDataCollector)
        {
            _fielHelper = fielHelper;
            _jsonFactory = jsonFactory;
            _embeddedContentRegexSettings = embeddedContentRegexSettings;
            _parsingSettings = parsingSettings;
            _paragraphUnitFactory = paragraphUnitFactory;
            _segmentDataCollector = segmentDataCollector;
        }

        public byte ProgressInPercent
        {
            get { return _progressInPercent; }
            set
            {
                _progressInPercent = value;
                OnProgress(_progressInPercent);
            }
        }

        public void SetFileProperties(IFileProperties properties)
        {
            Output.Initialize(DocumentProperties);

            var fileProperties = ItemFactory.CreateFileProperties();
            fileProperties.FileConversionProperties = properties.FileConversionProperties;
            Output.SetFileProperties(fileProperties);

            _paragraphUnitFactory.ItemFactory = ItemFactory;
            _paragraphUnitFactory.PropertiesFactory = PropertiesFactory;

            _fileConversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            _totalNumberOfLines = _fielHelper.GetNumberOfLines(_fileConversionProperties.OriginalFilePath);
            _reader = _jsonFactory.CreateJsonTextReader(_fileConversionProperties.OriginalFilePath);

            ProgressInPercent = 0;
        }

        public void EndOfInput()
        {
            _reader.Close();
            _reader.Dispose();
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _embeddedContentRegexSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
            _parsingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
        }

        public override bool ParseNext()
        {
            while (_reader.Read())
            {
                OnProgress((byte)(_reader.LineNumber * 100 / _totalNumberOfLines));

                var path = _reader.Path;
                var value = _reader.Value?.ToString() ?? string.Empty;

                if (_reader.TokenType != JsonToken.String)
                {
                    continue;
                }

                _segmentDataCollector.Add(path, value);
                var completeSegmentData = _segmentDataCollector.CompleteSegmentData;

                if (completeSegmentData == null)
                {
                    continue;
                }

                var paragraphUnit = _paragraphUnitFactory.Create(
                    completeSegmentData.SourcePath,
                    completeSegmentData.SourceValue,
                    completeSegmentData.TargetPath,
                    completeSegmentData.TargetValue);

                Output.ProcessParagraphUnit(paragraphUnit);

                return true;
            }

            return false;
        }
    }
}