using System;
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
        private IBilingualContentHandler _bilingualContentHandlerMock;
        private IUserSettings _userSettingsMock;
        private IDocumentItemFactory _itemFactoryMock;
        private IParagraphUnitFactory _paragraphUnitFactoryMock;

        public PoFileParserTest()
        {
            //Create here for performance reasons, not needed to be created for each test.
            _lineParsingSessionMock = A.Fake<ILineParsingSession>();
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgctxt ""The msgctxt text"""))
                .Returns(new ParseResult(LineType.MessageContext, "The msgctxt text", @"msgctxt ""The msgctxt text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid ""The msgid text"""))
                .Returns(new ParseResult(LineType.MessageId, "The msgid text", @"msgid ""The msgid text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid_plural ""The msgid_plural text"""))
                .Returns(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text", @"msgid_plural ""The msgid_plural text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid_plural """""))
                .Returns(new ParseResult(LineType.MessageIdPlural, "", @"msgid_plural """""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr ""The msgstr text"""))
                .Returns(new ParseResult(LineType.MessageString, "The msgstr text", @"msgstr ""The msgstr text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr[0] ""The msgstr[0] text"""))
                .Returns(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text", @"msgstr[0] ""The msgstr[0] text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr[0] """""))
                .Returns(new ParseResult(LineType.MessageStringPlural, "", @"msgstr[0] """""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr[1] ""The msgstr[1] text"""))
                .Returns(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text", @"msgstr[1] ""The msgstr[1] text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid """""))
                .Returns(new ParseResult(LineType.MessageId, "", @"msgid """""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr """""))
                .Returns(new ParseResult(LineType.MessageString, "", @"msgstr """""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid ""The text"""))
                .Returns(new ParseResult(LineType.MessageId, "The text", @"msgid ""The text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The text"""))
                .Returns(new ParseResult(LineType.Text, "The text", @"""The text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The second text"""))
                .Returns(new ParseResult(LineType.Text, "The second text", @"""The second text"""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"#: a comment"))
                .Returns(new ParseResult(LineType.Comment, "a comment", @"#: a comment"));
            A.CallTo(() => _lineParsingSessionMock.Parse(string.Empty))
                .Returns(new ParseResult(LineType.Empty, string.Empty, string.Empty));
            A.CallTo(() => _lineParsingSessionMock.Parse(MarkerLines.EndOfFile))
                .Returns(new ParseResult(LineType.EndOfFile, string.Empty, MarkerLines.EndOfFile));
        }

        [SetUp]
        public void SetUp()
        {
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            A.CallTo(() => _itemFactoryMock.PropertiesFactory).Returns(_propertiesFactoryMock);

            _bilingualContentHandlerMock = A.Fake<IBilingualContentHandler>();

            _userSettingsMock = A.Fake<IUserSettings>();

            _paragraphUnitFactoryMock = A.Fake<IParagraphUnitFactory>();
        }

        [Test]
        public void StartOfInput_ShouldSetTheProgressToZero()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            // Act
            testee.StartOfInput();

            // Assert
            testee.ProgressInPercent.Should().Be(0);
        }

        [Test]
        public void EndOfInput_ShouldInformContentHanlderAboutCompletion()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);
            testee.StartOfInput();

            // Act
            testee.EndOfInput();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.Complete()).MustHaveHappened();
            A.CallTo(() => _bilingualContentHandlerMock.FileComplete()).MustHaveHappened();
        }

        [Test]
        public void SetFileProperties_ShouldInitializeContentHandler()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

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
            var testee = CreateTestee(string.Empty);

            var filePropertiesMock = A.Fake<IFileProperties>();

            // Act
            testee.SetFileProperties(filePropertiesMock);

            // Assert
            _paragraphUnitFactoryMock.ItemFactory.Should().Be(_itemFactoryMock);
            _paragraphUnitFactoryMock.PropertiesFactory.Should().Be(_propertiesFactoryMock);
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
msgstr ""The msgstr text""";

            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var handler = A.Fake<EventHandler<ProgressEventArgs>>();
            testee.Progress += handler;

            // Act
            while (testee.ParseNext())
            {
            }

            //Assert
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 10)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 20)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 30)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 40)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 50)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 60)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 70)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 80)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 90)))
                .MustHaveHappened();
            A.CallTo(() => handler.Invoke(testee, A<ProgressEventArgs>.That.Matches(args => args.ProgressValue == 100)))
                .MustHaveHappened();
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
            testee.StartOfInput();

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
            testee.StartOfInput();
            testee.ParseNext();

            // Act
            var result = testee.ParseNext();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ParseNext_WhenMultipleEntries_ShouldParseOneEntryAtATime()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ParseNext_WhenMultipleEntries_ShouldParseOneTextAfterOther()
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

            // Act
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageId == "The msgid text"), A<LineType>.Ignored, A<bool>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ParseNext_WhenEntriesSeparatedByComment_ShouldRecognizeBothEntries()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
# this is a comment
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(A<IParagraphUnit>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ParseNext_WhenMsgidIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
                () =>
                    _paragraphUnitFactoryMock.Create(
                        A<Entry>.That.Matches(entry => entry.MessageId == "The msgid text"),
                        A<LineType>.Ignored,
                        A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
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

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
                () =>
                    _paragraphUnitFactoryMock.Create(
                        A<Entry>.That.Matches(entry => entry.MessageId == "The textThe second text"),
                        A<LineType>.Ignored,
                        A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
""The text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageId == "The msgid textThe text"),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageString == "The msgstr text"),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr """"
""The text""
""The second text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageString == "The textThe second text"),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
""The text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageString == "The msgstr textThe text"),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidIsSourceLineType_ShouldTakeMsgidAsSource()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            A.CallTo(() => _userSettingsMock.SourceLineType).Returns(LineType.MessageId);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.Ignored,
                       LineType.MessageId, 
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrIsSourceLineType_ShouldTakeMsgstrAsSource()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            A.CallTo(() => _userSettingsMock.SourceLineType).Returns(LineType.MessageString);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.Ignored,
                       LineType.MessageString,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidIsEmpty_ShouldIgnoreEntry()
        {
            // Arrange
            var testString = @"
msgid """"
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.Ignored,
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidOnOneLine_ShouldSetMsgidLocationToOneLine()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageIdStart == 2 && entry.MessageIdEnd == 2),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidOnMultipleLines_ShouldSetMsgidLocationToAllTheLines()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
""The text""
""The second text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageIdStart == 2 && entry.MessageIdEnd == 4),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrOnOneLine_ShouldSetMsgstrLocationToOneLine()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageStringStart == 3 && entry.MessageStringEnd == 3),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrOnMultipleLines_ShouldSetMsgstrContextToAllTheLines()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
""The text""
""The second text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(
               () =>
                   _paragraphUnitFactoryMock.Create(
                       A<Entry>.That.Matches(entry => entry.MessageStringStart == 3 && entry.MessageStringEnd == 5),
                       A<LineType>.Ignored,
                       A<bool>.Ignored))
               .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenEntryHasContext_ShouldCreateEntryWithContext()
        {
            // Arrange
            var testString = @"
msgctxt ""The msgctxt text""
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageContext == "The msgctxt text"), A<LineType>.Ignored, A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenEntryMsgidPlural_ShouldCreateEntryWithMsgidPlural()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgid_plural ""The msgid_plural text""
msgstr[0] ""The msgstr[0] text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageIdPlural == "The msgid_plural text"), A<LineType>.Ignored, A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenEntryMsgidPluralOnMultipleLines_ShouldCreateEntryWithMsgidPluralAndAllText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgid_plural """"
""The text""
""The second text""
msgstr[0] ""The msgstr[0] text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageIdPlural == "The textThe second text"), A<LineType>.Ignored, A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenEntryMsgstrPlural_ShouldCreateEntryWithMsgstrPlural()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgid_plural ""The msgid_plural text""
msgstr[0] ""The msgstr[0] text""
msgstr[1] ""The msgstr[1] text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageStringPlural[0] == "The msgstr[0] text" && entry.MessageStringPlural[1] == "The msgstr[1] text"), A<LineType>.Ignored, A<bool>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenEntryMsgstrPluralOnMultipleLines_ShouldCreateEntryWithMsgstrPluralWithAllText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgid_plural ""The msgid_plural text""
msgstr[0] """"
""The text""
""The second text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => _paragraphUnitFactoryMock.Create(A<Entry>.That.Matches(entry => entry.MessageStringPlural[0] == "The textThe second text"), A<LineType>.Ignored, A<bool>.Ignored))
                .MustHaveHappened();
        }

        private PoFileParser CreateTestee(string testString)
        {
            var fileHelperMock = A.Fake<IFileHelper>();

            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFilePath))
                .ReturnsNextFromSequence(
                    new ExtendedStreamReaderFake(testString),
                    new ExtendedStreamReaderFake(testString));

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineParsingSession()).Returns(_lineParsingSessionMock);

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFilePath);

            var filePropertiesMock = A.Fake<IFileProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(persistentFileConversionPropertiesMock);

            var testee = new PoFileParser(fileHelperMock, lineParserMock, _userSettingsMock, _paragraphUnitFactoryMock)
            {
                ItemFactory = _itemFactoryMock,
                Output = _bilingualContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}