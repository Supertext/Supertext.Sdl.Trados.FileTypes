using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IJsonFactory _jsonFactory;
        private readonly IFileHelper _fielHelper;
        private readonly IParsingSettings _parsingSettings;
        private readonly IEmbeddedContentRegexSettings _embeddedContentRegexSettings;

        //State during parsing
        private IPersistentFileConversionProperties _fileConversionProperties;
        private IJsonTextReader _reader;
        private int _totalNumberOfLines;

        public JsonFileParser(IJsonFactory jsonFactory, IFileHelper fielHelper, IEmbeddedContentRegexSettings embeddedContentRegexSettings, IParsingSettings parsingSettings)
        {
            _fielHelper = fielHelper;
            _jsonFactory = jsonFactory;
            _embeddedContentRegexSettings = embeddedContentRegexSettings;
            _parsingSettings = parsingSettings;
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
        }

        protected override void BeforeParsing()
        {
            OnProgress(0);

            _totalNumberOfLines = _fielHelper.GetNumberOfLines(_fileConversionProperties.OriginalFilePath);
            _reader = _jsonFactory.CreateJsonTextReader(_fileConversionProperties.OriginalFilePath);
        }

        protected override bool DuringParsing()
        {
            while (_reader.Read())
            {
                OnProgress((byte) (_reader.LineNumber*100/_totalNumberOfLines));

                var isPathToProcess = CheckIsPathToProcess(_reader.Path);

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

        private bool CheckIsPathToProcess(string path)
        {
            return !_parsingSettings.IsPathFilteringEnabled || _parsingSettings.PathPatterns.Select(pathPattern => new Regex(pathPattern).Match(path)).Any(match => match.Success);
        }

        protected override void AfterParsing()
        {
            _reader.Close();
            _reader.Dispose();
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