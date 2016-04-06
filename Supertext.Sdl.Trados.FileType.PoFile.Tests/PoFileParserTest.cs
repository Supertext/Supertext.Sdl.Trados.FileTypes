using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.ElementFactories;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileParserTest
    {
        private const string TestFilePath = "sample_file_ok";

        private ILineParser _lineParserMock;
        private ILineParsingSession _lineParsingSessionMock;
        private IEntryBuilder _entryBuilderMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private IBilingualContentHandler _bilingualContentHandlerMock;
        private ISegmentSettings _segmentSettingsMock;
        private IDocumentItemFactory _itemFactoryMock;
        private IParagraphUnitFactory _paragraphUnitFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            A.CallTo(() => _itemFactoryMock.PropertiesFactory).Returns(_propertiesFactoryMock);

            _bilingualContentHandlerMock = A.Fake<IBilingualContentHandler>();

            _segmentSettingsMock = A.Fake<ISegmentSettings>();

            _paragraphUnitFactoryMock = A.Fake<IParagraphUnitFactory>();

            _lineParserMock = A.Fake<ILineParser>();
            _entryBuilderMock = A.Fake<IEntryBuilder>();
            _lineParsingSessionMock = A.Fake<ILineParsingSession>();
            A.CallTo(() => _lineParserMock.StartLineParsingSession()).Returns(_lineParsingSessionMock);

            MathParseResultWithEntry(null, new ParseResult(LineType.Empty, string.Empty), null);

            MathParseResultWithEntry(@"entryComplete", new ParseResult(LineType.MessageString, "message string"),
                new Entry
                {
                    MessageId = "message id",
                    MessageString = "message string"
                });

            MathParseResultWithEntry(@"emptyMsgidEntryComplete", new ParseResult(LineType.MessageId, string.Empty),
                new Entry
                {
                    MessageId = string.Empty,
                    MessageString = "message string"
                });

            MathParseResultWithEntry(@"msgstrPluralEntryComplete",
                new ParseResult(LineType.MessageStringPlural, "message string 2"), new Entry
                {
                    MessageId = "message id",
                    MessageIdPlural = "message id plural",
                    MessageStringPlurals = new List<string>
                    {
                        "message string 0",
                        "message string 1",
                        "message string 2"
                    }
                });
        }

        private void MathParseResultWithEntry(string line, IParseResult parseResult, Entry completeEntry)
        {
            A.CallTo(() => _lineParsingSessionMock.Parse(line ?? A<string>.Ignored)).Returns(parseResult);

            A.CallTo(() => _entryBuilderMock.Add(parseResult, A<int>.Ignored))
                .Invokes(() => { A.CallTo(() => _entryBuilderMock.CompleteEntry).Returns(completeEntry); });
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
        public void InitializeSettings_ShouldSetSettingsByPopulatingFromSettingsBundle()
        {
            // Arrange
            var testString = string.Empty;
            var testee = CreateTestee(testString);
            const string configurationId = "testId";
            ISettingsBundle settingsBundleMock = A.Fake<ISettingsBundle>();

            // Act
            testee.InitializeSettings(settingsBundleMock, configurationId);

            // Assert
            A.CallTo(() => _segmentSettingsMock.PopulateFromSettingsBundle(settingsBundleMock, configurationId))
                .MustHaveHappened();
            A.CallTo(() =>_paragraphUnitFactoryMock.InitializeSettings(settingsBundleMock, configurationId))
                .MustHaveHappened();
        }

        [Test]
        public void ParseNext_WhenMultipleLines_ShouldUpdateProgressForEachLine()
        {
            // Arrange
            var testString = @"
msgid ""message id""
msgstr ""message string""

msgid ""message id""
msgstr ""message string""

msgid ""message id""
msgstr ""message string""";

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
        public void ParseNext_AsLongAsSomethingToParse_ShouldReturnTrue()
        {
            // Arrange
            var testString = @"
#: a comment
msgid ""message id""
msgstr ""message string""
entryComplete
msgid ""message id""
msgstr ""message string""
entryComplete
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            // Act
            var result = testee.ParseNext();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ParseNext_WhenNothingToParseAnymore_ShouldReturnFalse()
        {
            // Arrange
            var testString = @"
msgid ""message id""
msgstr ""message string""
entryComplete
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
msgid ""message id""
msgstr ""message string""
entryComplete
msgid ""message id""
msgstr ""message string""
entryComplete
msgid ""message id""
msgstr ""message string""
entryComplete
msgid ""message id""
msgstr ""message string""
entryComplete
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
        public void ParseNext_WhenMsgidIsEmpty_ShouldIgnoreEntry()
        {
            // Arrange
            var testString = @"
msgid """"
msgstr ""somehting like header""
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
        public void ParseNext_WhenMsgidIsSourceLineType_ShouldTakeMsgidAsSource()
        {
            // Arrange
            var testString = @"
msgid ""message id""
msgstr ""message string""
entryComplete
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            A.CallTo(() => _segmentSettingsMock.SourceLineType).Returns(LineType.MessageId);

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
msgid ""message id""
msgstr ""message string""
entryComplete
";
            var testee = CreateTestee(testString);
            testee.StartOfInput();

            A.CallTo(() => _segmentSettingsMock.SourceLineType).Returns(LineType.MessageString);

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

        private PoFileParser CreateTestee(string testString)
        {
            var fileHelperMock = A.Fake<IFileHelper>();

            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFilePath))
                .ReturnsNextFromSequence(
                    new ExtendedStreamReaderFake(testString),
                    new ExtendedStreamReaderFake(testString));

            var persistentFileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => persistentFileConversionPropertiesMock.OriginalFilePath).Returns(TestFilePath);

            var filePropertiesMock = A.Fake<IFileProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(persistentFileConversionPropertiesMock);

            var testee = new PoFileParser(fileHelperMock, _lineParserMock, _segmentSettingsMock,
                _paragraphUnitFactoryMock, _entryBuilderMock)
            {
                ItemFactory = _itemFactoryMock,
                Output = _bilingualContentHandlerMock
            };

            testee.SetFileProperties(filePropertiesMock);

            return testee;
        }
    }
}