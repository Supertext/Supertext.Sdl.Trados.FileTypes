using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly ISegmentReader _segmentReader;
        private readonly ILineParser _lineParser;
        private readonly IEntryBuilder _entryBuilder;
        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private IExtendedStreamReader _extendedStreamReader;
        private IStreamWriter _streamWriter;

        private ILineParsingSession _lineParsingSession;

        public PoFileWriter(IFileHelper fileHelper, ISegmentReader segmentReader, ILineParser lineParser, IEntryBuilder entryBuilder)
        {
            _fileHelper = fileHelper;
            _segmentReader = segmentReader;
            _lineParser = lineParser;
            _entryBuilder = entryBuilder;
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
        }

        public void SetFileProperties(IFileProperties fileInfo)
        {
            _extendedStreamReader = _fileHelper.GetExtendedStreamReader(_originalFileProperties.OriginalFilePath);
            _streamWriter = _fileHelper.GetStreamWriter(_nativeFileProperties.OutputFilePath);
            _lineParsingSession = _lineParser.StartLineParsingSession();
        }

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            var contextInfo = paragraphUnit.Properties.Contexts.Contexts[1];
            var messageStringStart = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageStringStart));
            var messageStringEnd = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageStringEnd));

            string currentOriginalLine;

            while ((currentOriginalLine = _extendedStreamReader.ReadLineWithEofLine()) != null)
            {
                if (_extendedStreamReader.CurrentLineNumber < messageStringStart)
                {
                    _streamWriter.WriteLine(currentOriginalLine);
                }

                var parseResult = _lineParsingSession.Parse(currentOriginalLine);

                _entryBuilder.Add(parseResult, _extendedStreamReader.CurrentLineNumber);

                if (_entryBuilder.CompleteEntry == null || string.IsNullOrEmpty(_entryBuilder.CompleteEntry.MessageId))
                {
                    continue;
                }

                //Todo refactor -> IsPluralForm
                if (_entryBuilder.CompleteEntry.MessageStringPlural.Count > 0)
                {
                    var segmentPairCounter = 0;
                    foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    {
                        _streamWriter.WriteLine("msgstr["+segmentPairCounter+"] \"" + _segmentReader.GetTargetText(segmentPair) + "\"");

                        segmentPairCounter++;
                    }
                }
                else
                {
                    var segmentPairCounter = 0;
                    foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    {
                        if (segmentPairCounter == 0)
                        {
                            _streamWriter.WriteLine("msgstr \"" + _segmentReader.GetTargetText(segmentPair) + "\"");

                        }
                        else
                        {
                            _streamWriter.WriteLine("\"" + _segmentReader.GetTargetText(segmentPair) + "\"");
                        }

                        segmentPairCounter++;
                    }
                }

                break;
            }

        }

        public void Complete()
        {
        }

        public void FileComplete()
        {
            _extendedStreamReader.Close();
            _extendedStreamReader.Dispose();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        public void Dispose()
        {
        }
    }
}