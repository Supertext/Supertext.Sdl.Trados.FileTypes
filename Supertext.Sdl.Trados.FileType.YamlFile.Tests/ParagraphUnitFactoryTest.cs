using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.YamlFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Tests
{
    [TestFixture]
    public class ParagraphUnitFactoryTest
    {
        private const string TheSourcePath = "the.source.path";
        private const string TheTargetPath = "the.target.path";
        private const string TheSourceValue = "The source value";
        private const string TheTargetValue = "The target value";
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private IParagraphUnit _paragraphUnitMock;

        [SetUp]
        public void SetUp()
        {
             _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(_paragraphUnitMock);
        }

        [Test]
        public void Create_ShouldAddFieldContextInfo()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var fieldContextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(StandardContextTypes.Paragraph))
                .Returns(fieldContextInfoMock);

            // Act
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, TheTargetValue);

            // Assert
            fieldContextInfoMock.Description.Should().Be(TheSourcePath);
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
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, TheTargetValue);

            // Assert
            A.CallTo(() => locationContextInfoMock.SetMetaData(ContextKeys.SourcePath, TheSourcePath)).MustHaveHappened();
            A.CallTo(() => locationContextInfoMock.SetMetaData(ContextKeys.TargetPath, TheTargetPath)).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(locationContextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenHasSourceAndTarget_ShouldAddSourceSegmentToParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(TheSourceValue)).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, TheTargetValue);

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphUnitMock.Source.Add(sourceSegmentMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenHasSourceOnly_ShouldAddSourceTextToParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(TheSourceValue)).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, string.Empty);

            // Assert
            A.CallTo(() => _paragraphUnitMock.Source.Add(textMock)).MustHaveHappened();
        }

        public void Create_WhenHasSourceAndTarget_ShouldAddTargetSegmentToParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(TheTargetValue)).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, TheTargetValue);

            // Assert
            A.CallTo(() => targetSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphUnitMock.Target.Add(targetSegmentMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenHasSourceOnly_ShouldNotAddTargetTextToParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(TheTargetValue)).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(TheSourcePath, TheSourceValue, TheTargetPath, string.Empty);

            // Assert
            A.CallTo(() => targetSegmentMock.Add(textMock)).MustNotHaveHappened();
            A.CallTo(() => _paragraphUnitMock.Target.Add(A<IAbstractMarkupData>.Ignored)).MustNotHaveHappened();
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