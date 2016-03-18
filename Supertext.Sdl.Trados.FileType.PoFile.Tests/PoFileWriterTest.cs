using FakeItEasy;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileWriterTest
    {
        private IStreamWriter _streamWriterMock;
        private const string TestFilePath = "sample_file_ok";

        [SetUp]
        public void SetUp()
        {
            _streamWriterMock = A.Fake<IStreamWriter>();
        }

        [Test]
        public void StructureTag_ShouldWriteTheHoleTagAsLine()
        {
            // Arrange
            var testee = CreateTestee();
            var structureTagPropertiesMock = A.Fake<IStructureTagProperties>();
            const string tagContent = "tag content";
            A.CallTo(() => structureTagPropertiesMock.TagContent).Returns(tagContent);

            testee.StartOfInput();

            // Act
            testee.StructureTag(structureTagPropertiesMock);

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine(tagContent)).MustHaveHappened();
        }

        [Test]
        public void Text_ShouldWriteTheText()
        {
            // Arrange
            var testee = CreateTestee();
            var textPropertiesMock = A.Fake<ITextProperties>();
            const string tagContent = "text content";
            A.CallTo(() => textPropertiesMock.Text).Returns(tagContent);

            testee.StartOfInput();

            // Act
            testee.Text(textPropertiesMock);

            // Assert
            A.CallTo(() => _streamWriterMock.Write(tagContent)).MustHaveHappened();
        }

        [Test]
        public void InlineStartTag_ShouldWriteTheTag()
        {
            // Arrange
            var testee = CreateTestee();
            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            const string tagContent = "tag content";
            A.CallTo(() => startTagPropertiesMock.TagContent).Returns(tagContent);

            testee.StartOfInput();

            // Act
            testee.InlineStartTag(startTagPropertiesMock);

            // Assert
            A.CallTo(() => _streamWriterMock.Write(tagContent)).MustHaveHappened();
        }

        [Test]
        public void InlineEndTag_ShouldWriteTheTag()
        {
            // Arrange
            var testee = CreateTestee();
            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            const string tagContent = "tag content";
            A.CallTo(() => endTagPropertiesMock.TagContent).Returns(tagContent);

            testee.StartOfInput();

            // Act
            testee.InlineEndTag(endTagPropertiesMock);

            // Assert
            A.CallTo(() => _streamWriterMock.Write(tagContent)).MustHaveHappened();
        }

        [Test]
        public void SegmentEnd_ShouldWriteALine()
        {
            // Arrange
            var testee = CreateTestee();

            testee.StartOfInput();

            // Act
            testee.SegmentEnd();

            // Assert
            A.CallTo(() => _streamWriterMock.WriteLine()).MustHaveHappened();
        }

        public PoFileWriter CreateTestee()
        {
            var fileHelperMock = A.Fake<IFileHelper>();
            A.CallTo(() => fileHelperMock.GetStreamWriter(TestFilePath)).Returns(_streamWriterMock);

            var nativeOutputFilePropertiesMock = A.Fake<INativeOutputFileProperties>();
            A.CallTo(() => nativeOutputFilePropertiesMock.OutputFilePath).Returns(TestFilePath);

            var testee = new PoFileWriter(fileHelperMock);
            testee.SetOutputProperties(nativeOutputFilePropertiesMock);

            return testee;
        }
    }
}
