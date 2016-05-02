using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileWriterTest
    {
        private IStreamWriter _streamWriterMock;
        private ISegmentReader _segmentReaderMock;
        private ILineParser _lineParserMock;
        private IEntryBuilder _entryBuilderMock;
        private ILineParsingSession _lineParsingSession;
        private ExtendedStreamReaderFake _extendedStreamReaderMock;
        private const string TestFileInputPath = "sample_input_file_ok";
        private const string TestFileOutputPath = "sample_output_file_ok";

        [SetUp]
        public void SetUp()
        {
            _streamWriterMock = A.Fake<IStreamWriter>();
            _segmentReaderMock = A.Fake<ISegmentReader>();
            _lineParserMock = A.Fake<ILineParser>();

            _entryBuilderMock = A.Fake<IEntryBuilder>();
            _lineParsingSession = A.Fake<ILineParsingSession>();
            A.CallTo(() => _lineParserMock.StartLineParsingSession()).Returns(_lineParsingSession);

            MathParseResultWithEntry(null, new ParseResult(LineType.Empty, string.Empty), null);

            MathParseResultWithEntry("entryComplete", new ParseResult(LineType.MessageString, "message string"), new Entry
            {
                MessageId = "message id",
                MessageString = "message string"
            });

            MathParseResultWithEntry("emptyMsgidEntryComplete", new ParseResult(LineType.MessageId, string.Empty), new Entry
            {
                MessageId = string.Empty,
                MessageString = "message string"
            });

            MathParseResultWithEntry("msgstrPluralEntryComplete", new ParseResult(LineType.MessageStringPlural, "message string 2"), new Entry
            {
                MessageId = "message id",
                MessageIdPlural = "message id plural",
                MessageStringPlurals = new List<string>
                    {
                        "message string 0",
                        "message string 1",
                        "message string 2"
                    }
            });

            A.CallTo(() => _segmentReaderMock.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string");
        }

        private void MathParseResultWithEntry(string line, IParseResult parseResult, Entry completeEntry)
        {
            A.CallTo(() => _lineParsingSession.Parse(line ?? A<string>.Ignored)).Returns(parseResult);

            A.CallTo(() => _entryBuilderMock.Add(parseResult, A<int>.Ignored)).Invokes(() =>
            {
                A.CallTo(() => _entryBuilderMock.CompleteEntry).Returns(completeEntry);
            });
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenStart_ShouldWriteLinesFromStartToMsgstr()
        {
            // Arrange
            var testString = @"line 1
line 2
msgid ""message id""
msgstr ""message string""
entryComplete
";
            var testee = CreateTestee(testString);

            var paragraphUnitMock = CreateParagraphUnitMock(4);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine("line 1")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 2")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgid ""message id""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenMultipleEntries_ShouldWriteLinesBetweenEntries()
        {
            // Arrange
            var testString = @"line 1
msgid ""message id""
msgstr ""message string""
entryComplete
line 5
msgid ""message id2""
msgstr ""message string2""
entryComplete
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock1 = CreateParagraphUnitMock(3);
            var paragraphUnitMock2 = CreateParagraphUnitMock(7);

            testee.ProcessParagraphUnit(paragraphUnitMock1);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock2);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine("entryComplete")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 5")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgid ""message id2""")).MustHaveHappened();
        }

        [Test]
        public void ProcessParagraphUnit_WhenMultipleEntries_ShouldWriteLinesInCorrectOrder()
        {
            // Arrange
            var testString = @"line 1
msgid ""message id""
msgstr ""message string""
entryComplete
line 5
msgid ""message id2""
msgstr ""message string2""
entryComplete
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock1 = CreateParagraphUnitMock(3);
            var paragraphUnitMock2 = CreateParagraphUnitMock(7);

            using (var scope = Fake.CreateScope())
            {
                // Act
                testee.ProcessParagraphUnit(paragraphUnitMock1);
                testee.ProcessParagraphUnit(paragraphUnitMock2);

                // Assert
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => _streamWriterMock.WriteLine("line 1")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine(@"msgid ""message id""")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""message string""")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine("entryComplete")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine("line 5")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine(@"msgid ""message id2""")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""message string""")).MustHaveHappened();
                    A.CallTo(() => _streamWriterMock.WriteLine("entryComplete")).MustHaveHappened();
                }
            }
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasOneMessageString_ShouldWriteMessageString()
        {
            // Arrange
            var testString = @"line 1
msgid ""message id""
msgstr ""message string""
entryComplete
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMock(3);

            

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasMessageStringOnMultipleLines_ShouldWriteMessageStringWithTextLines()
        {
            // Arrange
            var testString = @"line 1
line 2
msgid ""message id""
msgstr """"
""message string""
""message string""
""message string""
entryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMockForMessageStringWithText(4, 6);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr """"")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasMessageStringWithLineBreaks_ShouldWriteMessageStringWithTextLines()
        {
            // Arrange
            var testString = @"line 1
line 2
msgid ""message id""
msgstr """"
""message string\n""
""message string\n""
""message string\n""
entryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMockForMessageStringWithText(4, 4);

            A.CallTo(() => _segmentReaderMock.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string\nmessage string\nmessage string\n");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr """"")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string\n""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string\n""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string\n""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasMessageStringWithLineBreaksIncludingLastLine_ShouldNotWriteEmptyLineAtEnd()
        {
            // Arrange
            var testString = @"line 1
line 2
msgid ""message id""
msgstr """"
""message string\n""
""message string\n""
""message string\n""
entryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMockForMessageStringWithText(4, 4);

            A.CallTo(() => _segmentReaderMock.GetTargetText(A<ISegmentPair>.Ignored)).Returns("message string\nmessage string\nmessage string\n");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"""\n""")).MustNotHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasPluralForm_ShouldWriteMessageStringPlurals()
        {
            // Arrange
            var testString = @"line 1
msgid ""message id""
msgid_plural ""message id plural""
msgstr[0] ""message string""
msgstr[1] ""message string""
msgstr[2] ""message string""
msgstrPluralEntryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMockForMessageStringPlural(4, 6);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[0] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[1] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[2] ""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenEntryHasPluralFormAndTextLines_ShouldWriteMessageStringPluralsWithText()
        {
            // Arrange
            var testString = @"line 1
msgid ""message id""
msgid_plural ""message id plural""
msgstr[0] ""message string""
msgstr[1] ""message string""
msgstr[2] ""message string""
""message string""
msgstrPluralEntryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMockForMessageStringPluralWithText(4, 7, 6, 7);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[0] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[1] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr[2] ""message string""")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine(@"""message string""")).MustHaveHappened();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenMsgidIsEmpty_ShouldIgnoreEntry()
        {
            // Arrange
            var testString = @"emptyMsgidEntryComplete";

            var testee = CreateTestee(testString);
            var paragraphUnitMock = CreateParagraphUnitMock(2);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void FileComplete_ShouldCloseStreamReader()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            // Act
            testee.FileComplete();

            // Assert
            _extendedStreamReaderMock.Closed.Should().BeTrue();
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

            _extendedStreamReaderMock = new ExtendedStreamReaderFake(testString);
            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFileInputPath)).Returns(_extendedStreamReaderMock);
            A.CallTo(() => fileHelperMock.GetStreamWriter(TestFileOutputPath)).Returns(_streamWriterMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFileInputPath);

            var nativeOutputFilePropertiesMock = A.Fake<INativeOutputFileProperties>();
            A.CallTo(() => nativeOutputFilePropertiesMock.OutputFilePath).Returns(TestFileOutputPath);

            var outputFileInfoMock = A.Fake<IOutputFileInfo>();

            var filePropertiesMock = A.Fake<IFileProperties>();

            var testee = new PoFileWriter(fileHelperMock, _segmentReaderMock, _lineParserMock, _entryBuilderMock);
            testee.SetOutputProperties(nativeOutputFilePropertiesMock);
            testee.GetProposedOutputFileInfo(persistentFileConversionPropertiesMock, outputFileInfoMock);
            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }

        private static IParagraphUnit CreateParagraphUnitMock(int messageStringStartAndEnd)
        {
            var paragraphUnitPropertiesMock = CreateParagraphUnitPropertiesMock(messageStringStartAndEnd, messageStringStartAndEnd);

            var propertiesMock = A.Fake<ISegmentPairProperties>();
            A.CallTo(() => propertiesMock.Id).Returns(new SegmentId(1));
            var segmentPair = A.Fake<ISegmentPair>();
            A.CallTo(() => segmentPair.Properties).Returns(propertiesMock);
            var segmentPairs = new List<ISegmentPair> {segmentPair};

            return CreateParagraphUnitMock(segmentPairs, paragraphUnitPropertiesMock);
        }

        private static IParagraphUnit CreateParagraphUnitMockForMessageStringWithText(int messageStringStart, int messageStringEnd)
        {
            var paragraphUnitPropertiesMock = CreateParagraphUnitPropertiesMock(messageStringStart, messageStringEnd);

            var segmentPairs = new List<ISegmentPair>();

            var splittedIdSupplement = new[] { " a", " b", " c", " d" };
            var splittedIdSupplementCounter = 0;
            for (var i = messageStringStart; i <= messageStringEnd; i++)
            {
                var propertiesMock = A.Fake<ISegmentPairProperties>();
                A.CallTo(() => propertiesMock.Id).Returns(new SegmentId(i + splittedIdSupplement[splittedIdSupplementCounter++]));
                var segmentPair = A.Fake<ISegmentPair>();
                A.CallTo(() => segmentPair.Properties).Returns(propertiesMock);
                segmentPairs.Add(segmentPair);
            }

            return CreateParagraphUnitMock(segmentPairs, paragraphUnitPropertiesMock);
        }

        private static IParagraphUnit CreateParagraphUnitMockForMessageStringPlural(int messageStringStart, int messageStringEnd)
        {
            var paragraphUnitPropertiesMock = CreateParagraphUnitPropertiesMock(messageStringStart, messageStringEnd);

            var segmentPairs = new List<ISegmentPair>();

            for (var i = messageStringStart; i <= messageStringEnd; i++)
            {
                var propertiesMock = A.Fake<ISegmentPairProperties>();
                A.CallTo(() => propertiesMock.Id).Returns(new SegmentId(i));
                var segmentPair = A.Fake<ISegmentPair>();
                A.CallTo(() => segmentPair.Properties).Returns(propertiesMock);
                segmentPairs.Add(segmentPair);
            }

            return CreateParagraphUnitMock(segmentPairs, paragraphUnitPropertiesMock);
        }

        private static IParagraphUnit CreateParagraphUnitMockForMessageStringPluralWithText(int messageStringStart, int messageStringEnd, int pluralWithTextStart, int pluralWithTextEnd)
        {
            var paragraphUnitPropertiesMock = CreateParagraphUnitPropertiesMock(messageStringStart, messageStringEnd);

            var segmentPairs = new List<ISegmentPair>();

            var splittedIdSupplement = new[] { " a", " b", " c", " d" };
            var splittedIdSupplementCounter = 0;
            for (var i = messageStringStart; i <= messageStringEnd; i++)
            {
                var pluralStartIndex = i - pluralWithTextStart;

                var id = i >= pluralWithTextStart && i <=
                pluralWithTextEnd ? (i- pluralStartIndex) + splittedIdSupplement[splittedIdSupplementCounter++] : i.ToString();
                var propertiesMock = A.Fake<ISegmentPairProperties>();
                A.CallTo(() => propertiesMock.Id).Returns(new SegmentId(id));
                var segmentPair = A.Fake<ISegmentPair>();
                A.CallTo(() => segmentPair.Properties).Returns(propertiesMock);
                segmentPairs.Add(segmentPair);
            }

            return CreateParagraphUnitMock(segmentPairs, paragraphUnitPropertiesMock);
        }

        private static IParagraphUnitProperties CreateParagraphUnitPropertiesMock(int messageStringStart, int messageStringEnd)
        {
            var entryPositionsMock = A.Fake<IContextInfo>();
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MetaMessageStringStart))
                .Returns(messageStringStart.ToString());
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MetaMessageStringEnd))
                .Returns(messageStringEnd.ToString());

            var contextInfoMocks = new List<IContextInfo>
            {
                A.Fake<IContextInfo>(),
                entryPositionsMock
            };

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => contextPropertiesMock.Contexts).Returns(contextInfoMocks);

            var paragraphUnitPropertiesMock = A.Fake<IParagraphUnitProperties>();
            A.CallTo(() => paragraphUnitPropertiesMock.Contexts).Returns(contextPropertiesMock);
            return paragraphUnitPropertiesMock;
        }

        private static IParagraphUnit CreateParagraphUnitMock(List<ISegmentPair> segmentPairs,
            IParagraphUnitProperties paragraphUnitPropertiesMock)
        {
            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => paragraphUnitMock.SegmentPairs).Returns(segmentPairs);
            A.CallTo(() => paragraphUnitMock.Properties).Returns(paragraphUnitPropertiesMock);
            return paragraphUnitMock;
        }
    }
}
