using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Events;
using Newtonsoft.Json;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;


namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class JsonFileParserTest
    {
        private IJsonTextReader _jsonTextReaderMock;
        private INativeExtractionContentHandler _nativeExtractionContentHandlerMock;
        private ITextProperties _textPropertiesMock;
        private IContextInfo _contextInfoFieldMock;
        private IContextInfo _contextInfoPathMock;
        private IContextProperties _contextPropertiesMock;

        [SetUp]
        public void SetUp()
        {
            _jsonTextReaderMock = A.Fake<IJsonTextReader>();
            _nativeExtractionContentHandlerMock = A.Fake<INativeExtractionContentHandler>();
            _textPropertiesMock = A.Fake<ITextProperties>();
            _contextPropertiesMock = A.Fake<IContextProperties>();
            _contextInfoFieldMock = A.Fake<IContextInfo>();
            _contextInfoPathMock = A.Fake<IContextInfo>();
        }

        [Test]
        public void ParseNext_WhenFirstCalled_ShouldSetTheProgressToZero()
        {
            // Arrange
            var testee = CreateTestee();
            testee.MonitorEvents();

            // Act
            testee.ParseNext();

            // Assert
            testee.ShouldRaise("Progress").WithArgs<ProgressEventArgs>(args => args.ProgressValue == 0);
        }

        [Test]
        public void ParseNext_WhenParsing_ShouldUpdateTheProgressForEachLine()
        {
            // Arrange
            var testee = CreateTestee();
            testee.MonitorEvents();

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
        public void ParseNext_WhenLastCalled_ShouldCloseAndDisposeReader()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _jsonTextReaderMock.Close()).MustHaveHappened();
            A.CallTo(() => _jsonTextReaderMock.Dispose()).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathNotToProcess_ShouldLeavePathOut()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _jsonTextReaderMock.Path).Returns("not.process");

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(A<ITextProperties>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathToProcess_ShouldAddText()
        {
            // Arrange
            var testee = CreateTestee();
            
            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(_textPropertiesMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenPathToProcess_ShouldSetFieldContextWithPathAsDescription()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.ParseNext();

            // Assert
            _contextInfoFieldMock.Description.Should().Be("process");
            _contextPropertiesMock.Contexts.Should().Contain(_contextInfoFieldMock);
        }

        [Test]
        public void ParseNext_WhenPathToProcess_ShouldSetLocationContext()
        {
            // Arrange
            var testee = CreateTestee();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _contextInfoPathMock.SetMetaData(ContextKeys.ValuePath, "process")).MustHaveHappened();
            _contextPropertiesMock.Contexts.Should().Contain(_contextInfoPathMock);
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
            A.CallTo(() => _jsonTextReaderMock.Value).ReturnsLazily(() => texts[lineCounter-1 % 4]);
           
            A.CallTo(() => _jsonTextReaderMock.Path).Returns("process");

            var fileHelperMock = A.Fake<IFileHelper>();
            A.CallTo(() => fileHelperMock.GetNumberOfLines(A<string>.Ignored)).Returns(4);

            var embeddedContentRegexSettingsMock = A.Fake<IEmbeddedContentRegexSettings>();
            var parsingSettingsMock = A.Fake<IParsingSettings>();
            A.CallTo(() => parsingSettingsMock.PathPatterns).Returns(new List<string> { "^process$" });

            var filePropertiesMock = A.Fake<IFileProperties>();
            var propertiesFactoryMock = A.Fake<IPropertiesFactory>();
        
            A.CallTo(() => propertiesFactoryMock.CreateContextProperties()).Returns(_contextPropertiesMock);
            A.CallTo(() => propertiesFactoryMock.CreateContextInfo(StandardContextTypes.Field)).Returns(_contextInfoFieldMock);
            A.CallTo(() => propertiesFactoryMock.CreateContextInfo(ContextKeys.ValuePath)).Returns(_contextInfoPathMock);
            A.CallTo(() => propertiesFactoryMock.CreateTextProperties("A")).Returns(_textPropertiesMock);
            A.CallTo(() => _contextPropertiesMock.Contexts).Returns(new List<IContextInfo>());

            var testee = new JsonFileParser(jsonFactoryMock, fileHelperMock, embeddedContentRegexSettingsMock, parsingSettingsMock);
            testee.SetFileProperties(filePropertiesMock);
            testee.PropertiesFactory = propertiesFactoryMock;
            testee.Output = _nativeExtractionContentHandlerMock;
            return testee;
        }
    }
}
