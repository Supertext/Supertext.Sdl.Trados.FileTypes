using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;
using Supertext.Sdl.Trados.FileType.YamlFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Tests
{
    [TestFixture]
    public class YamlFileParserTest
    {
        private const string SourcePath = "The.Source.Path";
        private IYamlTextReader _yamlTextReaderMock;
        private IBilingualContentHandler _bilingualContentHandlerMock;
        private IEmbeddedContentRegexSettings _embeddedContentRegexSettingsMock;
        private IParsingSettings _parsingSettingsMock;
        private IParagraphUnitFactory _paragraphUnitFactoryMock;
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private ISegmentDataCollector _segmendDataCollectorMock;

        [SetUp]
        public void SetUp()
        {
            _yamlTextReaderMock = A.Fake<IYamlTextReader>();
            _bilingualContentHandlerMock = A.Fake<IBilingualContentHandler>();
            _embeddedContentRegexSettingsMock = A.Fake<IEmbeddedContentRegexSettings>();
            _parsingSettingsMock = A.Fake<IParsingSettings>();
            _paragraphUnitFactoryMock = A.Fake<IParagraphUnitFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _segmendDataCollectorMock = A.Fake<ISegmentDataCollector>();
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            A.CallTo(() => _itemFactoryMock.PropertiesFactory).Returns(_propertiesFactoryMock);
        }

        [Test]
        public void SetFileProperties_ShouldInitializeContentHandler()
        {
            // Arrange
            var testee = CreateTestee();

            var filePropertiesMock = A.Fake<IFileProperties>();

            // Act
            testee.SetFileProperties(filePropertiesMock);

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.Initialize(A<IDocumentProperties>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void SetFileProperties_ShouldSetParagraphUnitFactoryDependencies()
        {
            // Arrange
            var testee = CreateTestee();

            var filePropertiesMock = A.Fake<IFileProperties>();

            // Act
            testee.SetFileProperties(filePropertiesMock);

            // Assert
            _paragraphUnitFactoryMock.ItemFactory.Should().Be(_itemFactoryMock);
            _paragraphUnitFactoryMock.PropertiesFactory.Should().Be(_propertiesFactoryMock);
        }

        [Test]
        public void StartOfInput_ShouldSetTheProgressToZero()
        {
            // Arrange
            var testee = CreateTestee();
            testee.MonitorEvents();

            // Act
            testee.StartOfInput();

            // Assert
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 0);
        }

        [Test]
        public void EndOfInput_ShouldCloseAndDisposeReader()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            // Act
            testee.EndOfInput();

            // Assert
            A.CallTo(() => _yamlTextReaderMock.Close()).MustHaveHappened();
            A.CallTo(() => _yamlTextReaderMock.Dispose()).MustHaveHappened();
        }

        [Test]
        public void InitializeSettings_ShouldSetSettingsByPopulatingFromSettingsBundle()
        {
            // Arrange
            const string configurationId = "testId";
            var settingsBundleMock = A.Fake<ISettingsBundle>();
            var testee = CreateTestee();

            // Act
            testee.InitializeSettings(settingsBundleMock, configurationId);

            // Assert
            A.CallTo(() => _embeddedContentRegexSettingsMock.PopulateFromSettingsBundle(settingsBundleMock, configurationId))
                .MustHaveHappened();
            A.CallTo(() => _parsingSettingsMock.PopulateFromSettingsBundle(settingsBundleMock, configurationId))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenLastCalled_ShouldReturnFalse()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            var result = testee.ParseNext();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ParseNext_WhenParsing_ShouldUpdateTheProgressForEachLine()
        {
            // Arrange
            var testee = CreateTestee();
            testee.MonitorEvents();
            testee.StartOfInput();

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 0);
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 25);
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 50);
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 75);
            testee.ShouldRaise(nameof(testee.Progress)).WithArgs<ProgressEventArgs>(args => args.ProgressValue == 100);
        }

        [Test]
        public void ParseNext_WhenValueIsNull_ShouldNotProcessPath()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _yamlTextReaderMock.Value).Returns(null);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _segmendDataCollectorMock.Add(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenValueIsEmpty_ShouldNotProcessPath()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _yamlTextReaderMock.Value).Returns(string.Empty);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _segmendDataCollectorMock.Add(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_ShouldPassAllStringValuesToSegmentDataCollector()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _segmendDataCollectorMock.Add(SourcePath, "A")).MustHaveHappened();
            A.CallTo(() => _segmendDataCollectorMock.Add(SourcePath, "B")).MustHaveHappened();
            A.CallTo(() => _segmendDataCollectorMock.Add(SourcePath, "C")).MustHaveHappened();
            A.CallTo(() => _segmendDataCollectorMock.Add(SourcePath, "D")).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenCompleteSegmentDataIsNull_ShouldNotProcessParagraphUnit()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _segmendDataCollectorMock.CompleteSegmentData).Returns(null);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenCompleteSegmentDataIsNotNull_ShouldProcessParagraphUnit()
        {
            // Arrange
            var testSegmentData = new SegmentData
            {
                SourcePath = "SourcePath",
                SourceValue = "SourceValue",
                TargetPath = "TargetPath",
                TargetValue = "TargetValue"
            };
            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _segmendDataCollectorMock.CompleteSegmentData).Returns(testSegmentData);
            A.CallTo(() => _paragraphUnitFactoryMock.Create(
                testSegmentData.SourcePath,
                testSegmentData.SourceValue,
                testSegmentData.TargetPath,
                testSegmentData.TargetValue)).Returns(paragraphUnitMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(paragraphUnitMock)).MustHaveHappened();
        }

        private YamlFileParser CreateTestee()
        {
            var yamlFactory = A.Fake<IYamlFactory>();
            A.CallTo(() => yamlFactory.CreateYamlTextReader(A<string>.Ignored)).Returns(_yamlTextReaderMock);
            A.CallTo(() => _yamlTextReaderMock.Read()).ReturnsNextFromSequence(true, true, true, true, false);
            var lineCounter = 0;
            var texts = new[] { "A", "B", "C", "D" };
            A.CallTo(() => _yamlTextReaderMock.LineNumber).ReturnsLazily(() => ++lineCounter);
            A.CallTo(() => _yamlTextReaderMock.Value).ReturnsLazily(() => texts[lineCounter - 1 % 4]);

            A.CallTo(() => _yamlTextReaderMock.Path).Returns(SourcePath);

            var fileHelperMock = A.Fake<IFileHelper>();
            A.CallTo(() => fileHelperMock.GetNumberOfLines(A<string>.Ignored)).Returns(4);

            var filePropertiesMock = A.Fake<IFileProperties>();

            var testee = new YamlFileParser(
                yamlFactory,
                fileHelperMock,
                _embeddedContentRegexSettingsMock,
                _parsingSettingsMock,
                _paragraphUnitFactoryMock,
                _segmendDataCollectorMock)
            {
                ItemFactory = _itemFactoryMock,
                Output = _bilingualContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}
