using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IExtendedFileReader _extendedFileReader;
        private readonly ILineParser _lineParser;
        private readonly IUserSettings _userSettings;
        private IPersistentFileConversionProperties _fileConversionProperties;
        private ILineParsingSession _lineParsingSession;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _numberOfProcessedLines;

        public PoFileParser(IExtendedFileReader extendedFileReader, ILineParser lineParser, IUserSettings defaultUserSettings)
        {
            _extendedFileReader = extendedFileReader;
            _lineParser = lineParser;
            _userSettings = defaultUserSettings;
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
            _fileConversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
        }

        public void EndOfInput()
        {
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _userSettings.UpdateWith(settingsBundle, configurationId);
        }

        protected override void BeforeParsing()
        {
            _lineParsingSession = _lineParser.StartLineParsingSession();
            _totalNumberOfLines = _extendedFileReader.GetTotalNumberOfLines(_fileConversionProperties.OriginalFilePath);
            _numberOfProcessedLines = 0;

            ProgressInPercent = 0;
        }

        protected override bool DuringParsing()
        {
            var textExtractor = new TextExtractor(_userSettings.LineTypeToTranslate);

            foreach (var line in _extendedFileReader.ReadLinesWithEofLine(_fileConversionProperties.OriginalFilePath))
            {
                ProgressInPercent = (byte) (++_numberOfProcessedLines * 100 / _totalNumberOfLines);

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parseResult = _lineParsingSession.Parse(line);

                textExtractor.Process(parseResult);

                if (!textExtractor.IsTextComplete)
                {
                    continue;
                }

                Output.Text(PropertiesFactory.CreateTextProperties(textExtractor.Text));
                return true;
            }

            return false;
        }

        protected override void AfterParsing()
        {
            ProgressInPercent = 100;
        }

        private class TextExtractor
        {
            private readonly LineType _lineTypeToTranslate;
            private bool _processingText;

            public TextExtractor(LineType lineTypeToTranslate)
            {
                _lineTypeToTranslate = lineTypeToTranslate;
                _processingText = false;
            }

            public bool IsTextComplete { get; private set; }

            public string Text { get; private set; }

            public void Process(IParseResult parseResult)
            {
                if (parseResult.LineType != LineType.Text && _processingText)
                {
                    IsTextComplete = true;
                }
                else if (parseResult.LineType == LineType.Text && _processingText)
                {
                    Text += parseResult.LineContent;
                }
                else if (parseResult.LineType == _lineTypeToTranslate)
                {
                    Text += parseResult.LineContent;
                    _processingText = true;
                }
            }
        }
    }
}