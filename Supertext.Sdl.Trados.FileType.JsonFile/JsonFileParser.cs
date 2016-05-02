using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IEmbeddedContentRegexSettings _embeddedContentRegexSettings;
        private readonly TextProcessor _textProcessor;

        //State during parsing
        private StreamReader _file;
        private IPersistentFileConversionProperties _fileConversionProperties;
        private JsonTextReader _reader;
        private int _totalNumberOfLines;

        public JsonFileParser(TextProcessor textProcessor, IEmbeddedContentRegexSettings embeddedContentRegexSettings)
        {
            _textProcessor = textProcessor;
            _embeddedContentRegexSettings = embeddedContentRegexSettings;
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
            _embeddedContentRegexSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
            _textProcessor.InitializeWith(_embeddedContentRegexSettings.MatchRules);
        }

        protected override void BeforeParsing()
        {
            OnProgress(0);

            _totalNumberOfLines = File.ReadLines(_fileConversionProperties.OriginalFilePath).Count();
            _file = File.OpenText(_fileConversionProperties.OriginalFilePath);
            _reader = new JsonTextReader(_file);
        }

        protected override bool DuringParsing()
        {
            while (_reader.Read())
            {
                OnProgress((byte) (_reader.LineNumber*100/_totalNumberOfLines));

                var isPathToProcess = _reader.Path.EndsWith("html") ||
                                      _reader.Path.EndsWith("seo_description") ||
                                      _reader.Path.EndsWith("seo_keywords") ||
                                      _reader.Path.EndsWith("seo_title");

                if (_reader.Value == null || _reader.TokenType != JsonToken.String || !isPathToProcess)
                {
                    continue;
                }

                WriteContext(_reader.Path);
                WriteText(_reader.Value.ToString());

                return true;
            }

            return false;
        }

        protected override void AfterParsing()
        {
            _reader.Close();
            _file.Close();
            _file.Dispose();
        }

        private void WriteText(string value)
        {
            Output.Text(PropertiesFactory.CreateTextProperties(value));
        }

        private void WriteContext(string path)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();

            contextProperties.Contexts.Add(CreateFieldContextInfo(path));
            contextProperties.Contexts.Add(CreateLocationContextInfo(path));

            Output.ChangeContext(contextProperties);
        }

        private IContextInfo CreateFieldContextInfo(string path)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Field);
            contextInfo.Purpose = ContextPurpose.Match;
            contextInfo.Description = path;
            return contextInfo;
        }

        private IContextInfo CreateLocationContextInfo(string path)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(ContextKeys.ValuePath);
            contextInfo.Purpose = ContextPurpose.Location;
            contextInfo.DisplayName = JsonFileTypeResources.Value_Path;
            contextInfo.Description = JsonFileTypeResources.Value_Path;
            contextInfo.SetMetaData(ContextKeys.ValuePath, path);
            return contextInfo;
        }
    }
}