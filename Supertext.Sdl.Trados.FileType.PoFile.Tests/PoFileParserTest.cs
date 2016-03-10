using System;
using FakeItEasy;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileParserTest
    {
        private const string TestFilePath = "sample_file_ok";

        private ILineParsingSession _lineParsingSessionMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private INativeExtractionContentHandler _nativeExtractionContentHandlerMock;

        [SetUp]
        public void SetUp()
        {
            _lineParsingSessionMock = A.Fake<ILineParsingSession>();
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid ""The msgid text"""))
                .Returns(new ParseResult(LineType.MessageId, "The msgid text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr ""The msgstr text"""))
                .Returns(new ParseResult(LineType.MessageString, "The msgstr text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid """""))
                .Returns(new ParseResult(LineType.MessageId, ""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The text"""))
                .Returns(new ParseResult(LineType.Text, "The text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The second text"""))
                .Returns(new ParseResult(LineType.Text, "The second text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"#: a comment"))
                .Returns(new ParseResult(LineType.Comment, "a comment"));

            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _nativeExtractionContentHandlerMock = A.Fake<INativeExtractionContentHandler>();
        }

        [Test]
        public void ParseNext_ShouldIgnoreEmptyLines()
        {
            // Arrange
            var testString = @"

#: a comment
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""

";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _lineParsingSessionMock.Parse(string.Empty)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMultipleTextToTranslate_ShouldReturnOneTextWithEachCall()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var textPropertiesMock1 = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text")).Returns(textPropertiesMock1);

            // Act
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock1))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ParseNext_WhenMsgidNeedsToBeTranslated_ShouldAddMsgidText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text")).Returns(textPropertiesMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenTextToBeTranslatedIsOnMultipleLines_ShouldAddAllText()
        {
            // Arrange
            var testString = @"
msgid """"
""The text""
""The second text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The textThe second text")).Returns(textPropertiesMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }

        private PoFileParser CreateTestee(string testString)
        {
            var dotNetFactoryMock = A.Fake<IDotNetFactory>();

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineParsingSession()).Returns(_lineParsingSessionMock);

            var streamReaderFake = new StringReaderWrapper(testString);
            A.CallTo(() => dotNetFactoryMock.CreateStreamReader(TestFilePath)).Returns(streamReaderFake);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFilePath);

            var filePropertiesMock = A.Fake<IFileProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(persistentFileConversionPropertiesMock);

            var testee = new PoFileParser(dotNetFactoryMock, lineParserMock)
            {
                PropertiesFactory = _propertiesFactoryMock,
                Output = _nativeExtractionContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}