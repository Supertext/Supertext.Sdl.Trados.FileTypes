using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly ILineParser _lineParser;
        private readonly IUserSettings _userSettings;
        private IPersistentFileConversionProperties _fileConversionProperties;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private IExtendedStreamReader _extendedStreamReader;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _numberOfProcessedLines;
        private StringBuilder _textContent;
        private bool _processingText;

        public PoFileParser(IFileHelper fileHelper, ILineParser lineParser, IUserSettings defaultUserSettings)
        {
            _fileHelper = fileHelper;
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
            _userSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
        }

        protected override void BeforeParsing()
        {
            _lineParsingSession = _lineParser.StartLineParsingSession();
            _extendedStreamReader = _fileHelper.GetExtendedStreamReader(_fileConversionProperties.OriginalFilePath);
            _totalNumberOfLines = _fileHelper.GetExtendedStreamReader(_fileConversionProperties.OriginalFilePath).GetLinesWithEofLine().Count();
            _numberOfProcessedLines = 0;

            ProgressInPercent = 0;
        }

        protected override bool DuringParsing()
        {
            var currentLine = _extendedStreamReader.ReadLineWithEofLine();

            if (currentLine == null)
            {
                return false;
            }

            ProgressInPercent = (byte)(++_numberOfProcessedLines * 100 / _totalNumberOfLines);

            if (string.IsNullOrEmpty(currentLine))
            {
                WriteStructure(currentLine);
                return true;
            }

            var parseResult = _lineParsingSession.Parse(currentLine);

            if (parseResult.LineType == LineType.EndOfFile)
            {
                return false;
            }

            if (_processingText && parseResult.LineType == LineType.Text)
            {
                WriteText(parseResult.LineContent);
            }
            else if (parseResult.LineType == _userSettings.LineTypeToTranslate)
            {
                WriteText(parseResult.LineContent);
                _processingText = true;
            }
            else
            {
                _processingText = false;
                WriteStructure(currentLine);
            }

            return true;
        }

        protected override void AfterParsing()
        {
            _extendedStreamReader.Close();
            _extendedStreamReader.Dispose();
            _extendedStreamReader = null;
        }

        private void WriteStructure(string structureContent)
        {
            var structureTagProperties = PropertiesFactory.CreateStructureTagProperties(structureContent);
            structureTagProperties.DisplayText = structureContent;
            Output.StructureTag(structureTagProperties);
        }

        private void WriteText(string textContent)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(textContent);
            Output.Text(textProperties);
        }

    }
}