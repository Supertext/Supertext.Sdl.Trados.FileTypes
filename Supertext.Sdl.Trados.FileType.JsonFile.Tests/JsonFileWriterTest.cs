using FakeItEasy;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class JsonFileWriterTest
    {
        private const string TheSourcePath = "the.source.path";
        private const string TheTargetPath = "the.target.path";
        private const string TheTargetText = "the target text";
        private IFileHelper _fileHelperMock;
        private IJToken _rootTokenMock;
        private IJValue _targetValueMock;
        private IJValue _sourceValueMock;
        private IParagraphUnit _paragraphUnitMock;

        [SetUp]
        public void SetUp()
        {
            _fileHelperMock = A.Fake<IFileHelper>();
            _rootTokenMock = A.Fake<IJToken>();
            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            _sourceValueMock = A.Fake<IJValue>();
            _targetValueMock = A.Fake<IJValue>();
        }

        [Test]
        public void ProcessParagraphUnit_WhenTargetPathIsStringToken_ShouldReplaceValueWithTargetText()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallToSet(() => _targetValueMock.Value).To(TheTargetText).MustHaveHappened();

        }

        [Test]
        public void ProcessParagraphUnit_WhenTargetPathIsNotStringToken_ShouldNotReplaceValueWithTargetText()
        {
            // Arrange
            var testee = CreateTestee();

            var targetTokenMock = A.Fake<IJToken>();
            A.CallTo(() => _rootTokenMock.SelectToken(TheTargetPath)).Returns(targetTokenMock);

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallToSet(() => _targetValueMock.Value).To(TheTargetText).MustNotHaveHappened();

        }

        [Test]
        public void ProcessParagraphUnit_WhenTargetPathIsEmpty_ShouldTakeSourcePathAsTargetPath()
        {
            // Arrange
            var testee = CreateTestee(TheSourcePath, null);

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallToSet(() => _sourceValueMock.Value).To(TheTargetText).MustHaveHappened();
        }

        [Test]
        public void ProcessParagraphUnit_WhenValueIsNull_ShouldDoNothing()
        {
            // Arrange
            var testee = CreateTestee(TheSourcePath, null);
            var targetTokenMock = A.Fake<IJToken>();
            A.CallTo(() => _rootTokenMock.SelectToken(TheTargetPath)).Returns(targetTokenMock);
            A.CallTo(() => targetTokenMock.Type).Returns(JTokenType.String);
            A.CallTo(() => targetTokenMock.Value).Returns(null);

            // Act
            testee.ProcessParagraphUnit(_paragraphUnitMock);

            // Assert
            A.CallToSet(() => _targetValueMock.Value).To(TheTargetText).MustNotHaveHappened();
        }

        [Test]
        public void FileComplete_ShouldWriteJsonToOutputFile()
        {
            // Arrange
            const string theOutputPath = "the output path";
            const string theContent = "The content";
            var testee = CreateTestee();

            var outputFilePropertiesMock = A.Fake<INativeOutputFileProperties>();
            A.CallTo(() => outputFilePropertiesMock.OutputFilePath).Returns(theOutputPath);
            testee.SetOutputProperties(outputFilePropertiesMock);

            A.CallTo(() => _rootTokenMock.ToString()).Returns(theContent);

            // Act
            testee.FileComplete();

            // Assert
            A.CallTo(() => _fileHelperMock.WriteAllText(theOutputPath, theContent)).MustHaveHappened();

        }

        private JsonFileWriter CreateTestee(string sourcePath = TheSourcePath, string targetPath= TheTargetPath)
        {
            var jsonFactoryMock = A.Fake<IJsonFactory>();
           
            A.CallTo(() => jsonFactoryMock.GetRootToken(A<string>.Ignored)).Returns(_rootTokenMock);

            var filePropertiesMock = A.Fake<IFileProperties>();
            var fileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            var proposedFileInfoMock = A.Fake<IOutputFileInfo>();

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

            var sourceTokenMock = A.Fake<IJToken>();
            A.CallTo(() => _rootTokenMock.SelectToken(sourcePath)).Returns(sourceTokenMock);
            A.CallTo(() => sourceTokenMock.Type).Returns(JTokenType.String);
            A.CallTo(() => sourceTokenMock.Value).Returns(_sourceValueMock);

            var targetTokenMock = A.Fake<IJToken>();
            A.CallTo(() => _rootTokenMock.SelectToken(targetPath)).Returns(targetTokenMock);
            A.CallTo(() => targetTokenMock.Type).Returns(JTokenType.String);
            A.CallTo(() => targetTokenMock.Value).Returns(_targetValueMock);

            var segmentReaderMock = A.Fake<ISegmentReader>();
            A.CallTo(() => segmentReaderMock.GetText(_paragraphUnitMock.Target)).Returns(TheTargetText);

            var testee = new JsonFileWriter(jsonFactoryMock, _fileHelperMock, segmentReaderMock);
            testee.GetProposedOutputFileInfo(fileConversionPropertiesMock, proposedFileInfoMock);
            testee.SetFileProperties(filePropertiesMock);
            
            return testee;
        }
    }
}
