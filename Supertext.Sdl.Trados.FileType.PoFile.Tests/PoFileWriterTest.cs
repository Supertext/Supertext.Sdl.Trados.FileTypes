﻿using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileWriterTest
    {
        private IStreamWriter _streamWriterMock;
        private ISegmentReader _segmentReader;
        private ILineParser _lineParserMock;
        private IEntryBuilder _entryBuilderMock;
        private IExtendedStreamReader _extendedStreamReaderMock;
        private const string TestFileInputPath = "sample_input_file_ok";
        private const string TestFileOutputPath = "sample_output_file_ok";

        [SetUp]
        public void SetUp()
        {
            _streamWriterMock = A.Fake<IStreamWriter>();
            _segmentReader = A.Fake<ISegmentReader>();
            _lineParserMock = A.Fake<ILineParser>();
            _entryBuilderMock = new EntryBuilderFake();

            var lineParsingSession = A.Fake<ILineParsingSession>();
            A.CallTo(() => _lineParserMock.StartLineParsingSession()).Returns(lineParsingSession);
            A.CallTo(() => lineParsingSession.Parse(A<string>.Ignored))
                .Returns(new ParseResult(LineType.Empty, string.Empty, string.Empty));

            A.CallTo(() => lineParsingSession.Parse("msgstr end"))
                .Returns(new ParseResult(LineType.MessageString, "msgstr end", "msgstr"));

            A.CallTo(() => lineParsingSession.Parse("msgstr[2] end"))
                .Returns(new ParseResult(LineType.MessageStringPlural, "msgstr[2] end", "msgstr[2]"));
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenStart_ShouldWriteLinesFromStartToMsgstr()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3
line 4
line 5
";
            var testee = CreateTestee(testString);

            var paragraphUnitMock = CreateParagraphUnitMock(4, 4);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine("line 1")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 2")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 3")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenParagraphUnitAlreadyProcessed_ShouldWriteLinesFromProcessedToNewParagraphUnitWithMsgid()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3
msgstr end
line 5
line 6
line 7
msgstr end
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock1 = CreateParagraphUnitMock(4, 4);
            var paragraphUnitMock2 = CreateParagraphUnitMock(8, 8);

            testee.ProcessParagraphUnit(paragraphUnitMock1);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock2);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine("line 5")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 6")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 7")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasOneMessageString_ShouldWriteMessageString()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3
msgstr end
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMock(4, 4);

            A.CallTo(() => _segmentReader.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasOneMessageStringOnMultipleLines_ShouldWriteMessageStringWithTextLines()
        {
            // Arrange
            var testString = @"msgstr end";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMock(4, 6);

            A.CallTo(() => _segmentReader.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasPluralForm_ShouldWriteMessageStringPlurals()
        {
            // Arrange
            var testString = @"msgstr[2] end";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMock(4, 6);

            A.CallTo(() => _segmentReader.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[0] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[1] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[2] ""message string""")).MustHaveHappened();
        }

        [Test]
        public void FileComplete_ShouldCloseStreamReader()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            // Act
            testee.FileComplete();

            // Assert
            A.CallTo(() => _extendedStreamReaderMock.Close()).MustHaveHappened();
        }

        [Test]
        public void FileComplete_ShouldCloseStreamWriter()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            // Act
            testee.FileComplete();

            // Assert
            A.CallTo(() => _streamWriterMock.Close()).MustHaveHappened();
        }


        public PoFileWriter CreateTestee(string testString)
        {
            var fileHelperMock = A.Fake<IFileHelper>();

            _extendedStreamReaderMock = ExtendedStreamReaderFake.Create(testString);
            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFileInputPath)).Returns(_extendedStreamReaderMock);
            A.CallTo(() => fileHelperMock.GetStreamWriter(TestFileOutputPath)).Returns(_streamWriterMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFileInputPath);

            var nativeOutputFilePropertiesMock = A.Fake<INativeOutputFileProperties>();
            A.CallTo(() => nativeOutputFilePropertiesMock.OutputFilePath).Returns(TestFileOutputPath);

            var outputFileInfoMock = A.Fake<IOutputFileInfo>();

            var filePropertiesMock = A.Fake<IFileProperties>();

            var testee = new PoFileWriter(fileHelperMock, _segmentReader, _lineParserMock, _entryBuilderMock);
            testee.SetOutputProperties(nativeOutputFilePropertiesMock);
            testee.GetProposedOutputFileInfo(persistentFileConversionPropertiesMock, outputFileInfoMock);
            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }

        private static IParagraphUnit CreateParagraphUnitMock(int messageStringStart, int messageStringEnd)
        {
            var entryPositionsMock = A.Fake<IContextInfo>();
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageStringStart)).Returns(messageStringStart.ToString());
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageStringEnd)).Returns(messageStringEnd.ToString());

            var contextInfoMocks = new List<IContextInfo>
            {
                A.Fake<IContextInfo>(),
                entryPositionsMock
            };

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => contextPropertiesMock.Contexts).Returns(contextInfoMocks);

            var paragraphUnitPropertiesMock = A.Fake<IParagraphUnitProperties>();
            A.CallTo(() => paragraphUnitPropertiesMock.Contexts).Returns(contextPropertiesMock);

            var segmentPairCount = (messageStringEnd - messageStringStart) + 1;
            var segmentPairs = new List<ISegmentPair>();

            for (var i = 0; i < segmentPairCount; i++)
            {
                segmentPairs.Add(A.Fake<ISegmentPair>());
            }

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => paragraphUnitMock.SegmentPairs).Returns(segmentPairs);
            A.CallTo(() => paragraphUnitMock.Properties).Returns(paragraphUnitPropertiesMock);
            return paragraphUnitMock;

           
        }
    }

    public class EntryBuilderFake : IEntryBuilder
    {
        public Entry CompleteEntry { get; private set; }

        public void Add(IParseResult parseResult, int lineNumber)
        {
            CompleteEntry = null;

            if (parseResult.LineType == LineType.MessageString && parseResult.LineContent == "msgstr end")
            {
                CompleteEntry = new Entry
                {
                    MessageId = "message id",
                    MessageString = "message string"
                };
            }else if (parseResult.LineType == LineType.MessageStringPlural && parseResult.LineContent == "msgstr[2] end")
            {
                CompleteEntry = new Entry
                {
                    MessageId = "message id",
                    MessageIdPlural = "message id plural",
                    MessageStringPlural = new List<string>
                    {
                        "message string 0",
                        "message string 1",
                        "message string 2"
                    }
                };
            }


        }
    }
}
