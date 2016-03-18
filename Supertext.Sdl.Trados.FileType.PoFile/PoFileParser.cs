using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IExtendedStreamReader _extendedStreamReader;
        private readonly ILineParser _lineParser;
        private readonly IUserSettings _userSettings;
        private IPersistentFileConversionProperties _fileConversionProperties;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private Queue<string> _linesToProcess;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _numberOfProcessedLines;
        private StringBuilder _textContent;
        private bool _processingText;

        public PoFileParser(IExtendedStreamReader extendedStreamReader, ILineParser lineParser, IUserSettings defaultUserSettings)
        {
            _extendedStreamReader = extendedStreamReader;
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
            var linesWithEofLine = _extendedStreamReader.GetLinesWithEofLine(_fileConversionProperties.OriginalFilePath);
            _linesToProcess = new Queue<string>(linesWithEofLine);
            _totalNumberOfLines = _extendedStreamReader.GetTotalNumberOfLines(_fileConversionProperties.OriginalFilePath);
            _numberOfProcessedLines = 0;

            ProgressInPercent = 0;
        }

        protected override bool DuringParsing()
        {
            
            if (_linesToProcess.Count <= 0)
            {
                return false;
            }

            ProgressInPercent = (byte)(++_numberOfProcessedLines * 100 / _totalNumberOfLines);

            var currentLine = _linesToProcess.Dequeue();

            if (string.IsNullOrEmpty(currentLine))
            {
                WriteStructure(currentLine);
                return _linesToProcess.Count > 0;
            }

            var parseResult = _lineParsingSession.Parse(currentLine);

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

            return _linesToProcess.Count > 0;
        }

        protected override void AfterParsing()
        {
            ProgressInPercent = 100;
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