using System;
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
            _entryBuilderMock = A.Fake<IEntryBuilder>();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenStart_ShouldWriteLinesFromStartToMsgidWithMsgid()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3
line 4
line 5
";
            var testee = CreateTestee(testString);

            var paragraphUnitMock = CreateParagraphUnitMock("3", "3", "4", "4");

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
line 4
line 5
line 6
line 7
line 8
";
            var testee = CreateTestee(testString);
            var paragraphUnitMock1 = CreateParagraphUnitMock("3", "3", "4", "4");
            var paragraphUnitMock2 = CreateParagraphUnitMock("7", "7", "8", "8");

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
            ProcessParagraphUnit_ShouldWriteNewMsgstr()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3
line 4
line 5
";
            var testee = CreateTestee(testString);

            var paragraphUnitMock = CreateParagraphUnitMock("3", "3", "4", "4");

            A.CallTo(() => _segmentReader.GetTargetText(A<IEnumerable<ISegmentPair>>.Ignored)).Returns("sometext");

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(@"msgstr ""sometext""")).MustHaveHappened();
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

            _extendedStreamReaderMock = CreateExtendedStreamReaderMock(testString);
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

        private static IParagraphUnit CreateParagraphUnitMock(string messageIdStart, string messageIdEnd, string messageStringStart, string messageStringEnd)
        {
            var entryPositionsMock = A.Fake<IContextInfo>();
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageIdStart)).Returns(messageIdStart);
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageIdEnd)).Returns(messageIdEnd);
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageStringStart)).Returns(messageStringStart);
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageStringEnd)).Returns(messageStringEnd);

            var contextInfoMocks = new List<IContextInfo>
            {
                A.Fake<IContextInfo>(),
                entryPositionsMock
            };

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => contextPropertiesMock.Contexts).Returns(contextInfoMocks);

            var paragraphUnitPropertiesMock = A.Fake<IParagraphUnitProperties>();
            A.CallTo(() => paragraphUnitPropertiesMock.Contexts).Returns(contextPropertiesMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => paragraphUnitMock.Properties).Returns(paragraphUnitPropertiesMock);
            return paragraphUnitMock;
        }

        private static IExtendedStreamReader CreateExtendedStreamReaderMock(string testString)
        {
            var lines = (testString + Environment.NewLine + MarkerLines.EndOfFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var counter = 0;
            var extendedStreamReaderMock = A.Fake<IExtendedStreamReader>();
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).Returns(null);
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).ReturnsLazily(() =>
            {
                return lines[counter++];
            });
            A.CallTo(() => extendedStreamReaderMock.GetLinesWithEofLine()).ReturnsLazily(() =>
            {
                counter = lines.Length;
                return lines;
            });

            return extendedStreamReaderMock;
        }
    }
}
