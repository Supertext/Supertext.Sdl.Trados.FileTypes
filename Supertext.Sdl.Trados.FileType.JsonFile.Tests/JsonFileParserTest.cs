using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class JsonFileParserTest
    {
        private IJsonTextReader _jsonTextReaderMock;
        private IBilingualContentHandler _bilingualContentHandlerMock;
        private IEmbeddedContentRegexSettings _embeddedContentRegexSettingsMock;
        private IParsingSettings _parsingSettingsMock;
        private IParagraphUnitFactory _paragraphUnitFactoryMock;
        private IParagraphUnit _paragraphUnitMock;
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _jsonTextReaderMock = A.Fake<IJsonTextReader>();
            _bilingualContentHandlerMock = A.Fake<IBilingualContentHandler>();
            _embeddedContentRegexSettingsMock = A.Fake<IEmbeddedContentRegexSettings>();
            _parsingSettingsMock = A.Fake<IParsingSettings>();
            _paragraphUnitFactoryMock = A.Fake<IParagraphUnitFactory>();
            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            A.CallTo(() => _itemFactoryMock.PropertiesFactory).Returns(_propertiesFactoryMock);
            A.CallTo(() => _paragraphUnitFactoryMock.Create(_jsonTextReaderMock)).Returns(_paragraphUnitMock);
            A.CallTo(() => _parsingSettingsMock.IsPathFilteringEnabled).Returns(true);
            A.CallTo(() => _parsingSettingsMock.PathRules).Returns(new ComplexObservableList<PathRule>
            {
                new PathRule
                {
                    SourcePathPattern = "^process$"
                }
            });
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
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 0);
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
            A.CallTo(() => _jsonTextReaderMock.Close()).MustHaveHappened();
            A.CallTo(() => _jsonTextReaderMock.Dispose()).MustHaveHappened();
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
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 0);
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 25);
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 50);
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 75);
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 100);
        }

        [Test]
        public void ParseNext_WhenPathFilteringDisabledAndPathNotToProcess_ShouldProcessPath()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _jsonTextReaderMock.Path).Returns("not.process");
            A.CallTo(() => _parsingSettingsMock.IsPathFilteringEnabled).Returns(false);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathFilteringEnabledAndPathNotToProcess_ShouldLeavePathOut()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _jsonTextReaderMock.Path).Returns("not.process");

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathFilteringEnabledAndIgnoreCaseEnabled_ShouldIgnoreCasing()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            A.CallTo(() => _jsonTextReaderMock.Path).Returns("PROCESS");

            A.CallTo(() => _parsingSettingsMock.PathRules).Returns(new ComplexObservableList<PathRule>
            {
                new PathRule
                {
                    SourcePathPattern = "^process$",
                    IgnoreCase = true
                }
            });

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathToProcess_ShouldAddText()
        {
            // Arrange
            var testee = CreateTestee();
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(_paragraphUnitMock)).MustHaveHappened();
        }

        private JsonFileParser CreateTestee()
        {
            var jsonFactoryMock = A.Fake<IJsonFactory>();
            A.CallTo(() => jsonFactoryMock.CreateJsonTextReader(A<string>.Ignored)).Returns(_jsonTextReaderMock);
            A.CallTo(() => _jsonTextReaderMock.TokenType).Returns(JsonToken.String);
            A.CallTo(() => _jsonTextReaderMock.Read()).ReturnsNextFromSequence(true, true, true, true, false);
            var lineCounter = 0;
            var texts = new[] {"A", "B", "C", "D"};
            A.CallTo(() => _jsonTextReaderMock.LineNumber).ReturnsLazily(() => ++lineCounter);
            A.CallTo(() => _jsonTextReaderMock.Value).ReturnsLazily(() => texts[lineCounter - 1%4]);

            A.CallTo(() => _jsonTextReaderMock.Path).Returns("process");

            var fileHelperMock = A.Fake<IFileHelper>();
            A.CallTo(() => fileHelperMock.GetNumberOfLines(A<string>.Ignored)).Returns(4);

            var filePropertiesMock = A.Fake<IFileProperties>();

            var testee = new JsonFileParser(jsonFactoryMock, fileHelperMock, _embeddedContentRegexSettingsMock,
                _parsingSettingsMock, _paragraphUnitFactoryMock)
            {
                ItemFactory = _itemFactoryMock,
                Output = _bilingualContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}