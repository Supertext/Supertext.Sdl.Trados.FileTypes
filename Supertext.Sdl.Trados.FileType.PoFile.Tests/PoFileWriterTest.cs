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
        private IStreamReader _streamReaderMock;
        private IStreamWriter _streamWriterMock;
        private const string TestFileInputPath = "sample_input_file_ok";
        private const string TestFileOutputPath = "sample_output_file_ok";

        [SetUp]
        public void SetUp()
        {
            _streamReaderMock = A.Fake<IStreamReader>();
            _streamWriterMock = A.Fake<IStreamWriter>();
        }

        [Test]
        public void
            ProcessParagraphUnit_WhenStart_ShouldWriteLinesFromStartToMsgidPosition()
        {
            // Arrange
            var testee = CreateTestee();
            var inputLines = @"line 1
line 2
line 3
line 4
line 5
";
            A.CallTo(() => _streamReaderMock.ReadLine()).Returns(null);
            A.CallTo(() => _streamReaderMock.ReadLine()).ReturnsNextFromSequence(inputLines.Split(new[] { Environment.NewLine },
                StringSplitOptions.None));

            var entryPositionsMock = A.Fake<IContextInfo>();
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageIdStart)).Returns("3");
            A.CallTo(() => entryPositionsMock.GetMetaData(ContextKeys.MessageIdEnd)).Returns("3");

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

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine("line 1")).MustHaveHappened();
            A.CallTo(() => _streamWriterMock.WriteLine("line 2")).MustHaveHappened();
        }

        public PoFileWriter CreateTestee()
        {
            var fileHelperMock = A.Fake<IFileHelper>();
            A.CallTo(() => fileHelperMock.GetStreamReader(TestFileInputPath)).Returns(_streamReaderMock);
            A.CallTo(() => fileHelperMock.GetStreamWriter(TestFileOutputPath)).Returns(_streamWriterMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFileInputPath);

            var nativeOutputFilePropertiesMock = A.Fake<INativeOutputFileProperties>();
            A.CallTo(() => nativeOutputFilePropertiesMock.OutputFilePath).Returns(TestFileOutputPath);

            var outputFileInfoMock = A.Fake<IOutputFileInfo>();

            var filePropertiesMock = A.Fake<IFileProperties>();

            var testee = new PoFileWriter(fileHelperMock);
            testee.SetOutputProperties(nativeOutputFilePropertiesMock);
            testee.GetProposedOutputFileInfo(persistentFileConversionPropertiesMock, outputFileInfoMock);
            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}
