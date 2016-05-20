using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class ParagraphUnitFactoryTest
    {
        private const string TheTestPath = "the.test.path";
        private const string TheTestValue = "The test value";
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private IParagraphUnit _paragraphUnitMock;
        private IJsonTextReader _defaultReader;

        [SetUp]
        public void SetUp()
        {
             _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(_paragraphUnitMock);
            _defaultReader = A.Fake<IJsonTextReader>();
            A.CallTo(() => _defaultReader.Value).Returns(TheTestValue);
            A.CallTo(() => _defaultReader.Path).Returns(TheTestPath);
        }

        [Test]
        public void Create_ShouldAddFieldContextInfo()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var fieldContextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(StandardContextTypes.Field))
                .Returns(fieldContextInfoMock);

            // Act
            testee.Create(_defaultReader);

            // Assert
            fieldContextInfoMock.Description.Should().Be(TheTestPath);
            A.CallTo(() => contextPropertiesMock.Contexts.Add(fieldContextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void Create_ShouldAddLocationContextInfo()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var locationContextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.ValueLocation))
                .Returns(locationContextInfoMock);

            // Act
            testee.Create(_defaultReader);

            // Assert
            A.CallTo(() => locationContextInfoMock.SetMetaData(ContextKeys.SourcePath, TheTestPath)).MustHaveHappened();
            A.CallTo(() => locationContextInfoMock.SetMetaData(ContextKeys.TargetPath, TheTestPath)).MustHaveHappened();
        }

        [Test]
        public void Create_ShouldAddSourceSegmentToParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();

            var sourceSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(sourceSegmentMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(TheTestValue)).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(_defaultReader);

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphUnitMock.Source.Add(sourceSegmentMock)).MustHaveHappened();
        }

        private ParagraphUnitFactory CreateTestee()
        {
            var paragraphUnitFactory = new ParagraphUnitFactory
            {
                ItemFactory = _itemFactoryMock,
                PropertiesFactory = _propertiesFactoryMock
            };

            return paragraphUnitFactory;
        }
    }
}