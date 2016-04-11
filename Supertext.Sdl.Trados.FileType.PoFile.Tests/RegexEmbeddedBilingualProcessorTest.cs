using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class RegexEmbeddedBilingualProcessorTest
    {
        private IEmbeddedContentVisitorFactory _embeddedContentVisitorFactoryMock;
        private IEmbeddedContentRegexSettings _embeddedContetnRegexSettingsMock;
        private ITextProcessor _textProcessorMock;
        private ISettingsBundle _settingsBundleMock;
        private IDocumentItemFactory _itemFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _embeddedContentVisitorFactoryMock = A.Fake<IEmbeddedContentVisitorFactory>();
            _embeddedContetnRegexSettingsMock = A.Fake<IEmbeddedContentRegexSettings>();
            _textProcessorMock = A.Fake<ITextProcessor>();
            _settingsBundleMock = A.Fake<ISettingsBundle>();
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
        }

        [Test]
        public void InitializeSettings_ShouldInitializeTextProcessor()
        {
            // Arrange
            var matchRules = new ComplexObservableList<MatchRule>();
            A.CallTo(() => _embeddedContetnRegexSettingsMock.MatchRules).Returns(matchRules);

            var testee = CreateTestee();

            // Act
            testee.InitializeSettings(_settingsBundleMock, string.Empty);

            // Assert
            A.CallTo(() => _textProcessorMock.InitializeWith(matchRules)).MustHaveHappened();
        }

        [Test]
        public void InitializeSettings_ShouldInitializeSettings()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.InitializeSettings(_settingsBundleMock, string.Empty);

            // Assert
            A.CallTo(() => _embeddedContetnRegexSettingsMock.PopulateFromSettingsBundle(_settingsBundleMock, string.Empty)).MustHaveHappened();
        }

        [Test]
        public void ProcessParagraphUnit_WhenSettingsIsEnabled_UpdateSource()
        {
            // Arrange
            var testee = CreateTestee();

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            var sourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(sourceMock);

            var newSourceMock = A.Fake<IParagraph>();
            var abstractMarkupDataMock = A.Fake<IAbstractMarkupData>();
            A.CallTo(() => newSourceMock.Count).ReturnsNextFromSequence(1, 0);
            A.CallTo(() => newSourceMock[0]).Returns(abstractMarkupDataMock);
            var embeddedContentVisitor = A.Fake<IEmbeddedContentVisitor>();
            A.CallTo(() => embeddedContentVisitor.GeneratedParagraph).Returns(newSourceMock);
            A.CallTo(() => _embeddedContentVisitorFactoryMock.CreateVisitor(A<IDocumentItemFactory>.Ignored, A<IPropertiesFactory>.Ignored, _textProcessorMock)).Returns(embeddedContentVisitor);

            A.CallTo(() => _embeddedContetnRegexSettingsMock.IsEnabled).Returns(true);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => sourceMock.Clear()).MustHaveHappened();
            A.CallTo(() => sourceMock.Add(abstractMarkupDataMock)).MustHaveHappened();
        }

        [Test]
        public void ProcessParagraphUnit_WhenSettingsIsEnabled_UpdateTarget()
        {
            // Arrange
            var testee = CreateTestee();

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            var targetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(targetMock);

            var newTargetMock = A.Fake<IParagraph>();
            var abstractMarkupDataMock = A.Fake<IAbstractMarkupData>();
            A.CallTo(() => newTargetMock.Count).ReturnsNextFromSequence(1, 0, 1, 0);
            A.CallTo(() => newTargetMock[0]).Returns(abstractMarkupDataMock);
            var embeddedContentVisitor = A.Fake<IEmbeddedContentVisitor>();
            A.CallTo(() => embeddedContentVisitor.GeneratedParagraph).Returns(newTargetMock);
            A.CallTo(() => _embeddedContentVisitorFactoryMock.CreateVisitor(A<IDocumentItemFactory>.Ignored, A<IPropertiesFactory>.Ignored, _textProcessorMock)).Returns(embeddedContentVisitor);

            A.CallTo(() => _embeddedContetnRegexSettingsMock.IsEnabled).Returns(true);

            // Act
            testee.ProcessParagraphUnit(paragraphUnitMock);

            // Assert
            A.CallTo(() => targetMock.Clear()).MustHaveHappened();
            A.CallTo(() => targetMock.Add(abstractMarkupDataMock)).MustHaveHappened();
        }

        private RegexEmbeddedBilingualProcessor CreateTestee()
        {
            var testee = new RegexEmbeddedBilingualProcessor(_embeddedContentVisitorFactoryMock, _embeddedContetnRegexSettingsMock, _textProcessorMock);
            testee.ItemFactory = _itemFactoryMock;
            return testee;
        }
    }
}
