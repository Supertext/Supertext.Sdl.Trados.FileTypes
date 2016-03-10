using System;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : INativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IDotNetFactory _dotNetFactory;
        private readonly ILineParser _lineParser;
        private IPersistentFileConversionProperties _fileConversionProperties;
        private IReader _reader;
        private ILineParsingSession _lineParsingSession;

        public PoFileParser(IDotNetFactory dotNetFactory, ILineParser lineParser)
        {
            _dotNetFactory = dotNetFactory;
            _lineParser = lineParser;
        }

        public event EventHandler<ProgressEventArgs> Progress;

        public IPropertiesFactory PropertiesFactory { get; set; }

        public INativeContentStreamMessageReporter MessageReporter { get; set; }

        public INativeExtractionContentHandler Output { get; set; }

        public void SetFileProperties(IFileProperties properties)
        {
            _fileConversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            _reader = _dotNetFactory.CreateStreamReader(_fileConversionProperties.OriginalFilePath);
            _lineParsingSession = _lineParser.StartLineParsingSession();
        }

        public void EndOfInput()
        {
            _reader.Close();
        }

        public bool ParseNext()
        {
            string currentLine;
            var textExtractor = new TextExtractor();

            while ((currentLine = _reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    continue;
                }

                var parseResult = _lineParsingSession.Parse(currentLine);

                textExtractor.Process(parseResult);

                if (textExtractor.IsTextComplete)
                {
                    Output.Text(PropertiesFactory.CreateTextProperties(textExtractor.Text));
                    return true;
                }
            }

            return false;
        }

        private class TextExtractor
        {
            private readonly StringBuilder _textBuilder;
            private bool _processingText;

            public TextExtractor()
            {
                _textBuilder = new StringBuilder();
                _processingText = false;
            }

            public bool IsTextComplete { get; private set; }

            public string Text { get; private set; }

            public void Process(IParseResult parseResult)
            {
                if (parseResult.LineType != LineType.Text && _processingText)
                {
                    Text = _textBuilder.ToString();
                    IsTextComplete = true;
                }
                else if (parseResult.LineType == LineType.Text && _processingText)
                {
                    _textBuilder.Append(parseResult.LineContent);
                }
                else if (parseResult.LineType == LineType.MessageId)
                {
                    _textBuilder.Append(parseResult.LineContent);
                    _processingText = true;
                }
            }
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            throw new NotImplementedException();
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
    }
}