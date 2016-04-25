using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private const string MessageStringKeyword = @"msgstr ""{0}""";
        private const string MessageStringPluralKeyword = @"msgstr[{1}] ""{0}""";
        private const string MessageStringText = @"""{0}""";
        private const string SplittedSegmentIdNextPattern = @"\d+\s[b-z]+";

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
            return string.Format(MessageStringPluralKeyword, lineContent, segmentPairCounter);
        }

        private static string CreateMessageStringLine(string lineContent)
        {
            return string.Format(MessageStringKeyword, lineContent);
        }

        private static string CreateTextLine(string lineContent)
        {
            return string.Format(MessageStringText, lineContent);
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

                var parseResult = _lineParsingSession.Parse(currentOriginalLine);

                _entryBuilder.Add(parseResult, _extendedStreamReader.CurrentLineNumber);

                var completeEntry = _entryBuilder.CompleteEntry;

                if (completeEntry == null || string.IsNullOrEmpty(completeEntry.MessageId))
                {
                    continue;
                }

                WriteMessageString(paragraphUnit, completeEntry.IsPluralForm);
                
                if (_extendedStreamReader.CurrentLineNumber > messageStringEnd && parseResult.LineType != LineType.EndOfFile)
                {
                    _streamWriter.WriteLine(currentOriginalLine);
                }

                break;
            }
        }

        private void WriteMessageString(IParagraphUnit paragraphUnit, bool isPluralForm)
        {
            var segmentPairsLines = GetSegmentPairsLines(paragraphUnit.SegmentPairs);

            if (isPluralForm)
            {
                WriteMessageStringPluralLines(segmentPairsLines);
            }
            else
            {
                WriteMessageStringLines(segmentPairsLines);
            }
        }

        private void WriteMessageStringPluralLines(Dictionary<string, List<string>> segmentPairsLines)
        {
            var segmentPairCounter = 0;

            foreach (var segmentPairLines in segmentPairsLines)
            {
                var isFirstLine = true;
                foreach (var segmentPairLine in segmentPairLines.Value)
                {
                    var toWrite = isFirstLine && !_splittedSegmentIdNextRegex.IsMatch(segmentPairLines.Key)
                        ? CreateMessageStringPluralLine(segmentPairCounter, segmentPairLine)
                        : CreateTextLine(segmentPairLine);

                    _streamWriter.WriteLine(toWrite);

                    isFirstLine = false;
                }

                segmentPairCounter++;
            }
        }

        private void WriteMessageStringLines(Dictionary<string, List<string>> segmentPairsLines)
        {
            var firstSegmentPairLines = segmentPairsLines.First();

            if (segmentPairsLines.Count > 1 || firstSegmentPairLines.Value.Count > 1)
            {
                var messageStringLine = CreateMessageStringLine(string.Empty);
                _streamWriter.WriteLine(messageStringLine);

                foreach (var segmentPairLines in segmentPairsLines)
                {
                    foreach (var segmentPairLine in segmentPairLines.Value)
                    {
                        _streamWriter.WriteLine(CreateTextLine(segmentPairLine));
                    }
                }
            }
            else
            {
                _streamWriter.WriteLine(CreateMessageStringLine(firstSegmentPairLines.Value[0]));
            }
        }

        private Dictionary<string, List<string>> GetSegmentPairsLines(IEnumerable<ISegmentPair> segmentPairs)
        {
            var allLines = new Dictionary<string, List<string>> ();

            foreach (var segmentPair in segmentPairs)
            {
                var targetText = _segmentReader.GetTargetText(segmentPair);

                var lines = GetLines(targetText);

                foreach (var line in lines)
                {
                    if (!allLines.ContainsKey(segmentPair.Properties.Id.Id))
                    {
                        allLines.Add(segmentPair.Properties.Id.Id, new List<string>());
                    }

                    allLines[segmentPair.Properties.Id.Id].Add(line);
                }
            }

            return allLines;
        }

        private IEnumerable<string> GetLines(string targetText)
        {
            if (!targetText.Contains("\n"))
            {
                return new List<string>
                {
                    targetText
                };
            }

            var lines = targetText.Split(new[] {"\n"},
                StringSplitOptions.None).Select(line => line + "\\n").ToList();

            if (lines.Last() == "\\n")
            {
                lines.RemoveAt(lines.Count-1);
            }

            return lines;
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