using FakeItEasy;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;
using Supertext.Sdl.Trados.FileType.YamlFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Tests
{
    [TestFixture]
    public class YamlFileWriterTest
    {
        private const string TheSourcePath = "the.source.path";
        private const string TheTargetPath = "the.target.path";
        private const string TheTargetText = "the target text";
        private IParagraphUnit _paragraphUnitMock;
        private IYamlTextWriter _yamlTextWriter;

        [SetUp]
        public void SetUp()
        {
            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            _yamlTextWriter = A.Fake<IYamlTextWriter>();
        }

        [Test]
        public void ProcessParagraphUnit_WhenTargetPathIsEmpty_ShouldTakeSourcePathAsTargetPath()
        {
            // Arrange
            var testee = CreateTestee(TheSourcePath, null);

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallTo(() => _yamlTextWriter.Write(TheSourcePath, A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void ProcessParagraphUnit_ShouldPassTargetPathAndTargetTextToYamlTextWriter()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallTo(() => _yamlTextWriter.Write(TheTargetPath, TheTargetText)).MustHaveHappened();
        }

        [Test]
        public void FileComplete_ShouldCallYamlTextWriterFinishWriting()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.FileComplete();

            // Assert
            A.CallTo(() => _yamlTextWriter.FinishWriting()).MustHaveHappened();

        }

        private YamlFileWriter CreateTestee(string sourcePath = TheSourcePath, string targetPath= TheTargetPath)
        {
            var jsonFactoryMock = A.Fake<IYamlFactory>();
            A.CallTo(() => jsonFactoryMock.CreateYamlTextWriter(A<string>.Ignored, A<string>.Ignored))
                .Returns(_yamlTextWriter);

            var filePropertiesMock = A.Fake<IFileProperties>();
            var fileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            var proposedFileInfoMock = A.Fake<IOutputFileInfo>();
            var nativeOutputPropertiesMock = A.Fake<INativeOutputFileProperties>();

            var paragraphUnitPropertiesMock = A.Fake<IParagraphUnitProperties>();
            var contextPropertiesMock = A.Fake<IContextProperties>();
            var fieldContextInfoMock = A.Fake<IContextInfo>();
            var locationContextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => paragraphUnitPropertiesMock.Contexts).Returns(contextPropertiesMock);
            A.CallTo(() => contextPropertiesMock.Contexts[0]).Returns(fieldContextInfoMock);
            A.CallTo(() => contextPropertiesMock.Contexts[1]).Returns(locationContextInfoMock);
            A.CallTo(() => locationContextInfoMock.GetMetaData(ContextKeys.TargetPath)).Returns(targetPath);
            A.CallTo(() => locationContextInfoMock.GetMetaData(ContextKeys.SourcePath)).Returns(sourcePath);
            A.CallTo(() => _paragraphUnitMock.Properties).Returns(paragraphUnitPropertiesMock);

            var segmentReaderMock = A.Fake<ISegmentReader>();
            A.CallTo(() => segmentReaderMock.GetText(_paragraphUnitMock.Target)).Returns(TheTargetText);

            var testee = new YamlFileWriter(jsonFactoryMock, segmentReaderMock);
            testee.SetOutputProperties(nativeOutputPropertiesMock);
            testee.GetProposedOutputFileInfo(fileConversionPropertiesMock, proposedFileInfoMock);
            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}
