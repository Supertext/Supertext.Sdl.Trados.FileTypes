using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.ElementFactories;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private const string MessageStringKeyword = "msgstr";
        private const string MessageStringPluralKeyword = "msgstr[{0}]";
        private const string SplittedSegmentIdStartPattern = @"\d+\sa";
        private const string SplittedSegmentIdNextPattern = @"\d+\s[a-z]+";

        private readonly Regex _splittedSegmentIdStartRegex = new Regex(SplittedSegmentIdStartPattern);
        private readonly Regex _splittedSegmentIdNextRegex = new Regex(SplittedSegmentIdNextPattern);
        private readonly IFileHelper _fileHelper;
        private readonly ISegmentReader _segmentReader;
        private readonly ILineParser _lineParser;
        private readonly IEntryBuilder _entryBuilder;
        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private IExtendedStreamReader _extendedStreamReader;
        private IStreamWriter _streamWriter;
        private ILineParsingSession _lineParsingSession;

        public PoFileWriter(IFileHelper fileHelper, ISegmentReader segmentReader, ILineParser lineParser,
            IEntryBuilder entryBuilder)
        {
            _fileHelper = fileHelper;
            _segmentReader = segmentReader;
            _lineParser = lineParser;
            _entryBuilder = entryBuilder;
        }

        private static string CreateMessageStringPluralLine(int segmentPairCounter, string lineContent)
        {
            return string.Format(MessageStringPluralKeyword, segmentPairCounter) + " \"" + lineContent + "\"";
        }

        private static string CreateMessageStringLine(string lineContent)
        {
            return MessageStringKeyword + " \"" + lineContent + "\"";
        }

        private static string CreateTextLine(string lineContent)
        {
            return "\"" + lineContent + "\"";
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

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties,
            IOutputFileInfo proposedFileInfo)
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
            var messageStringStart = int.Parse(contextInfo.GetMetaData(ContextKeys.MetaMessageStringStart));
            var messageStringEnd = int.Parse(contextInfo.GetMetaData(ContextKeys.MetaMessageStringEnd));

            string currentOriginalLine;

            while ((currentOriginalLine = _extendedStreamReader.ReadLineWithEofLine()) != null)
            {
                if (_extendedStreamReader.CurrentLineNumber < messageStringStart)
                {
                    _streamWriter.WriteLine(currentOriginalLine);
                }

                var completeEntry = GetCompleteEntry(currentOriginalLine);

                if (completeEntry == null || string.IsNullOrEmpty(completeEntry.MessageId))
                {
                    continue;
                }

                WriteMessageString(paragraphUnit, completeEntry.IsPluralForm);
                
                if (_extendedStreamReader.CurrentLineNumber > messageStringEnd)
                {
                    _streamWriter.WriteLine(currentOriginalLine);
                }

                break;
            }
        }

        private Entry GetCompleteEntry(string currentOriginalLine)
        {
            var parseResult = _lineParsingSession.Parse(currentOriginalLine);

            _entryBuilder.Add(parseResult, _extendedStreamReader.CurrentLineNumber);

            return _entryBuilder.CompleteEntry;
        }

        private void WriteMessageString(IParagraphUnit paragraphUnit, bool isPluralForm)
        {
            var segmentPairCounter = 0;
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                var lineContent = _segmentReader.GetTargetText(segmentPair);

                string toWrite;

                if (_splittedSegmentIdStartRegex.IsMatch(segmentPair.Properties.Id.Id) || !_splittedSegmentIdNextRegex.IsMatch(segmentPair.Properties.Id.Id))
                {
                    toWrite = isPluralForm ? CreateMessageStringPluralLine(segmentPairCounter, lineContent) : CreateMessageStringLine(lineContent);
                }
                else
                {
                    toWrite = CreateTextLine(lineContent);
                }

                _streamWriter.WriteLine(toWrite);

                segmentPairCounter++;
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