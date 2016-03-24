using System;
using System.Linq;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
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
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgstr """""))
                .Returns(new ParseResult(LineType.MessageString, ""));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"msgid ""The text"""))
                .Returns(new ParseResult(LineType.MessageId, "The text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The text"""))
                .Returns(new ParseResult(LineType.Text, "The text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"""The second text"""))
                .Returns(new ParseResult(LineType.Text, "The second text"));
            A.CallTo(() => _lineParsingSessionMock.Parse(@"#: a comment"))
                .Returns(new ParseResult(LineType.Comment, "a comment"));
            A.CallTo(() => _lineParsingSessionMock.Parse(string.Empty))
                .Returns(new ParseResult(LineType.Empty, string.Empty));
            A.CallTo(() => _lineParsingSessionMock.Parse(MarkerLines.EndOfFile))
                .Returns(new ParseResult(LineType.EndOfFile, string.Empty));

            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            A.CallTo(() => _itemFactoryMock.PropertiesFactory).Returns(_propertiesFactoryMock);

            _bilingualContentHandlerMock = A.Fake<IBilingualContentHandler>();

            _userSettingsMock = A.Fake<IUserSettings>();
            A.CallTo(() => _userSettingsMock.SourceLineType).Returns(LineType.MessageId);
            A.CallTo(() => _userSettingsMock.IsTargetTextNeeded).Returns(true);
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text")).Returns(textPropertiesMock);

            var msgidTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgidTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var sourceSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(sourceSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(msgidTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphSourceMock.Add(sourceSegmentMock)).MustHaveHappened();
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The textThe second text"))
                .Returns(textPropertiesMock);

            var msgidTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgidTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var sourceSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(sourceSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(msgidTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphSourceMock.Add(sourceSegmentMock)).MustHaveHappened();
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid textThe text"))
                .Returns(textPropertiesMock);

            var msgidTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgidTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var sourceSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(sourceSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(msgidTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphSourceMock.Add(sourceSegmentMock)).MustHaveHappened();
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgstr text")).Returns(textPropertiesMock);

            var msgstrTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgstrTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(targetSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => targetSegmentMock.Add(msgstrTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphTargetMock.Add(targetSegmentMock)).MustHaveHappened();
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The textThe second text"))
                .Returns(textPropertiesMock);

            var msgstrTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgstrTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(targetSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => targetSegmentMock.Add(msgstrTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphTargetMock.Add(targetSegmentMock)).MustHaveHappened();
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

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgstr textThe text"))
                .Returns(textPropertiesMock);

            var msgstrTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(msgstrTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored)).Returns(targetSegmentMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => targetSegmentMock.Add(msgstrTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphTargetMock.Add(targetSegmentMock)).MustHaveHappened();
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

            var msgidTextProperties = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text"))
                .Returns(msgidTextProperties);

            var msgstrTextProperties = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgstr text"))
                .Returns(msgstrTextProperties);

            var msgidTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(msgidTextProperties)).Returns(msgidTextMock);

            var msgstrTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(msgstrTextProperties)).Returns(msgstrTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored))
                .ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            A.CallTo(() => _userSettingsMock.SourceLineType).Returns(LineType.MessageString);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => sourceSegmentMock.Add(msgstrTextMock)).MustHaveHappened();
            A.CallTo(() => paragraphSourceMock.Add(sourceSegmentMock)).MustHaveHappened();
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
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties(string.Empty)).MustNotHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidOnOneLine_ShouldSetMsgidLocationContextToOneLine()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.LocationContextType))
                .Returns(contextInfoMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageIdStart, "2")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageIdEnd, "2")).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgidOnMultipleLines_ShouldSetMsgidLocationContextToAllTheLines()
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

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.LocationContextType))
                .Returns(contextInfoMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageIdStart, "2")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageIdEnd, "4")).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrOnOneLine_ShouldSetMsgstrLocationContextToOneLine()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.LocationContextType))
                .Returns(contextInfoMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringStart, "3")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringEnd, "3")).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMsgstrOnMultipleLines_ShouldSetMsgstrLocationContextToAllTheLines()
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

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.LocationContextType))
                .Returns(contextInfoMock);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringStart, "3")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringEnd, "5")).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void ParseNext_ShouldProcessOneParagraphUnitPerEntry()
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

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            // Act
            testee.ParseNext();
            testee.ParseNext();

            // Assert
            A.CallTo(() => _bilingualContentHandlerMock.ProcessParagraphUnit(paragraphUnitMock))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ParseNext_WhenTargetIsNotNeeded_ShouldNotAddTargetText()
        {
            // Arrange
            var testString = @"
msgid ""The msgid text""
msgstr ""The msgstr text""
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            var msgidTextProperties = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgid text"))
                .Returns(msgidTextProperties);

            var msgstrTextProperties = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("The msgstr text"))
                .Returns(msgstrTextProperties);

            var msgidTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(msgidTextProperties)).Returns(msgidTextMock);

            var msgstrTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(msgstrTextProperties)).Returns(msgstrTextMock);

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(paragraphUnitMock);

            var paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Source).Returns(paragraphSourceMock);

            var paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => paragraphUnitMock.Target).Returns(paragraphTargetMock);

            var sourceSegmentMock = A.Fake<ISegment>();
            var targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored))
                .ReturnsNextFromSequence(sourceSegmentMock, targetSegmentMock);

            A.CallTo(() => _userSettingsMock.IsTargetTextNeeded).Returns(false);

            // Act
            testee.ParseNext();

            // Assert
            A.CallTo(() => targetSegmentMock.Add(msgstrTextMock)).MustNotHaveHappened();
            A.CallTo(() => paragraphSourceMock.Add(targetSegmentMock)).MustNotHaveHappened();
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

            var testee = new PoFileParser(fileHelperMock, lineParserMock, _userSettingsMock)
            {
                ItemFactory = _itemFactoryMock,
                Output = _bilingualContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}