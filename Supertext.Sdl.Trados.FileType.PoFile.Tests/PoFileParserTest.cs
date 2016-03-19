﻿using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

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
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid ""The text"""))
                .Returns(new ParseResult(LineType.MessageId, "The text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The text"""))
                .Returns(new ParseResult(LineType.Text, "The text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The second text"""))
                .Returns(new ParseResult(LineType.Text, "The second text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"#: a comment"))
                .Returns(new ParseResult(LineType.Comment, "a comment"));
            A.CallTo(() => _lineParsingSessionMock.Parse(MarkerLines.EndOfFile))
                .Returns(new ParseResult(LineType.EndOfFile, string.Empty));

            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _nativeExtractionContentHandlerMock = A.Fake<INativeExtractionContentHandler>();

            _userSettingsMock = A.Fake<IUserSettings>();
            A.CallTo(() => _userSettingsMock.LineTypeToTranslate).Returns(LineType.MessageId);
        }

        [Test]
        public void ParseNext_ShouldReturnTrueAsLongAsSomethingToParse()
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
            testee.ParseNext();
            testee.ParseNext();

            // Act
            var result = testee.ParseNext();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ParseNext_ShouldReturnFalseWhenNothingToParseAnymore()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Act
            var result = testee.ParseNext();

            // Assert
            result.Should().BeFalse();
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
            while (testee.ParseNext())
            {
            }

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
            testee.ParseNext();
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
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenTextToBeTranslatedIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid """"
""The text""
""The second text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var textPropertiesMock1 = A.Fake<ITextProperties>();
            var textPropertiesMock2 = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The text")).Returns(textPropertiesMock1);
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The second text")).Returns(textPropertiesMock2);

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock1)).MustHaveHappened();
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock2)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenTextToBeTranslatedIsOnMultipleLines_ShouldNotTakeEmptyTextLine()
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
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("")).Returns(textPropertiesMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenTextToBeTranslatedIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid ""The text""
""The second text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var textPropertiesMock1 = A.Fake<ITextProperties>();
            var textPropertiesMock2 = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The text")).Returns(textPropertiesMock1);
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The second text")).Returns(textPropertiesMock2);

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock1)).MustHaveHappened();
            A.CallTo(() => _nativeExtractionContentHandlerMock.Text(textPropertiesMock2)).MustHaveHappened();
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
            while (testee.ParseNext()) { }

            //Assert
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 0))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 10))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 20))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 30))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 40))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 50))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 60))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 70))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 80))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 90))).MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 100))).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMultipleLines_ShouldProcessNotTranslatableTextAsStructure()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);

            var structureTagProperties = A.Fake<IStructureTagProperties>();
            var structureTagProperties2 = A.Fake<IStructureTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStructureTagProperties(string.Empty)).Returns(structureTagProperties);
            A.CallTo(() => _propertiesFactoryMock.CreateStructureTagProperties(@"msgstr ""The msgstr text""")).Returns(structureTagProperties2);

            // Act
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _nativeExtractionContentHandlerMock.StructureTag(structureTagProperties)).MustHaveHappened();
            A.CallTo(() => _nativeExtractionContentHandlerMock.StructureTag(structureTagProperties2)).MustHaveHappened();
        }

        private PoFileParser CreateTestee(string testString)
        {
            var fileHelperMock = A.Fake<IFileHelper>();

            var lines = testString.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var extendedStreamReaderMock = A.Fake<IExtendedStreamReader>();
            A.CallTo(() => fileHelperMock.GetTotalNumberOfLines(TestFilePath)).Returns(lines.Length);
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).Returns(null);
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).Returns(MarkerLines.EndOfFile);
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).ReturnsNextFromSequence(lines);
            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFilePath)).Returns(extendedStreamReaderMock);

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineParsingSession()).Returns(_lineParsingSessionMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFilePath);

            var filePropertiesMock = A.Fake<IFileProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(persistentFileConversionPropertiesMock);

            var testee = new PoFileParser(fileHelperMock, lineParserMock, _userSettingsMock)
            {
                PropertiesFactory = _propertiesFactoryMock,
                Output = _nativeExtractionContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}