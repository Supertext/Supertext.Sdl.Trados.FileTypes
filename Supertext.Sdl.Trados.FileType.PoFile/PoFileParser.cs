using System;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : AbstractNativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly ILineParser _lineParser;
        private IPersistentFileConversionProperties _fileConversionProperties;
        private IReader _reader;
        private ILineParsingSession _lineParsingSession;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _numberOfProcessedLines;

        public PoFileParser(IFileHelper fileHelper, ILineParser lineParser)
        {
            _fileHelper = fileHelper;
            _lineParser = lineParser;
        }

        public bool IsMessageIdSource { get; private set; }

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
            IsMessageIdSource = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _reader == null)
            {
                return;
            }

            _reader.Dispose();
            _reader = null;
        }

        protected override void BeforeParsing()
        {
            _reader = _fileHelper.CreateStreamReader(_fileConversionProperties.OriginalFilePath);
            _lineParsingSession = _lineParser.StartLineParsingSession();

            _totalNumberOfLines = _fileHelper.GetTotalNumberOfLines(_fileConversionProperties.OriginalFilePath);
            _numberOfProcessedLines = 0;

            ProgressInPercent = 0;
        }

        protected override bool DuringParsing()
        {
            string currentLine;
            var textExtractor = new TextExtractor();

            while ((currentLine = _reader.ReadLine()) != null)
            {
                ProgressInPercent = (byte) ((++_numberOfProcessedLines * 100) / _totalNumberOfLines);

                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    continue;
                }

                var parseResult = _lineParsingSession.Parse(currentLine);

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
            _reader.Close();
            ProgressInPercent = 100;
        }

        private class TextExtractor
        {
            private bool _processingText;

            public TextExtractor()
            {
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
                else if (parseResult.LineType == LineType.MessageId)
                {
                    Text += parseResult.LineContent;
                    _processingText = true;
                }
            }
        }
    }
}