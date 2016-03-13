using System;
using System.Collections.Generic;
using System.Text;
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
        private IUserSettings _userSettingsMock;

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

            _userSettingsMock = A.Fake<IUserSettings>();
            A.CallTo(() => _userSettingsMock.LineTypeToTranslate).Returns(LineType.MessageId);
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text")).Returns(textPropertiesMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrNeedsToBeTranslated_ShouldAddMsgstrText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgstr text")).Returns(textPropertiesMock);

            A.CallTo(() => _userSettingsMock.LineTypeToTranslate).Returns(LineType.MessageString);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenTextToBeTranslatedIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid """"
""The text""
""The second text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The textThe second text")).Returns(textPropertiesMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }
        
        [Test]
        public void ParseNext_WhenFirstCalled_ShouldResetProgressToZero()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            var handler = A.Fake<EventHandler<ProgressEventArgs>>();
            testee.Progress += handler;

            // Act
            testee.ParseNext();

            //Assert
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 0))).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMultipleLines_ShouldUpdateProgressForEachLine()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var handler = A.Fake<EventHandler<ProgressEventArgs>>();
            testee.Progress += handler;

            // Act
            testee.ParseNext();

            //Assert
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 0))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 10))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 20))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 30))).MustHaveHappened();
        }

        private PoFileParser CreateTestee(string testString)
        {
            var fileHelper = A.Fake<IFileHelper>();
            var lines = testString.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            A.CallTo(() => fileHelper.GetTotalNumberOfLines(TestFilePath)).Returns(lines.Length);
            A.CallTo(() => fileHelper.ReadLines(TestFilePath)).Returns(lines);

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineParsingSession()).Returns(_lineParsingSessionMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFilePath);

            var filePropertiesMock = A.Fake<IFileProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(persistentFileConversionPropertiesMock);

            var testee = new PoFileParser(fileHelper, lineParserMock, _userSettingsMock)
            {
                PropertiesFactory = _propertiesFactoryMock,
                Output = _nativeExtractionContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}