using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class PoFileSnifferTest
    {
        private const string TestFilePath = "sample_file_ok";

        private Language _testLanguage;
        private Codepage _testCodepage;
        private INativeTextLocationMessageReporter _messageReporterMock;
        private ISettingsGroup _settingsGroupMock;
        private ILineValidationSession _lineValidationSessionMock;


        [SetUp]
        public void SetUp()
        {
            _testLanguage = new Language();
            _testCodepage = new Codepage();
            _messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            _settingsGroupMock = A.Fake<ISettingsGroup>();
            _lineValidationSessionMock = A.Fake<ILineValidationSession>();
        }

        [Test]
        public void Sniff_WhenAllLinesAreValid_ShouldReturnIsSupportedTrue()
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
            A.CallTo(() => _lineValidationSessionMock.IsValid(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid(MarkerLines.EndOfFile)).Returns(true);

            // Act
            var result = testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeTrue();
        }

        [Test]
        public void Sniff_WhenEndIsInvalid_ShouldReturnIsSupportedFalse()
        {
            // Arrange
            var testString = @"
#: a comment
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
";
            var testee = CreateTestee(testString);
            A.CallTo(() => _lineValidationSessionMock.IsValid(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid(MarkerLines.EndOfFile)).Returns(false);

            // Act
            var result = testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenEndIsInvalid_ShouldReportExpectedPattern()
        {
            // Arrange
            var testString = @"
#: a comment
msgid ""The msgid text""
msgstr ""The msgstr text""

msgid ""The msgid text""
";
            var testee = CreateTestee(testString);
            A.CallTo(() => _lineValidationSessionMock.IsValid(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid(MarkerLines.EndOfFile)).Returns(false);
            A.CallTo(() => _lineValidationSessionMock.NextExpectedLineDescription).Returns("msgstr");

            // Act
            testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            A.CallTo(() => _messageReporterMock.ReportMessage(
                testee,
                TestFilePath,
                ErrorLevel.Error,
                A<string>.That.Contains("msgstr"),
                A<string>.Ignored
                )).MustHaveHappened();
        }

        [Test]
        public void Sniff_WhenOneLineIsInvalid_ShouldReturnIsSupportedFalse()
        {
            // Arrange
            var testString = @"
#: a comment
msgid ""The msgid text""
somethingwrong
";
            var testee = CreateTestee(testString);
            A.CallTo(() => _lineValidationSessionMock.IsValid("#: a comment")).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid(@"msgid ""The msgid text""")).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid("somethingwrong")).Returns(false);

            // Act
            var result = testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenOneLineIsInvalid_ShouldReportWhichLineHasError()
        {
            // Arrange
            var testString = @"
#: a comment
msgid ""The msgid text""
somethingwrong
";

            var testee = CreateTestee(testString);
            A.CallTo(() => _lineValidationSessionMock.IsValid("#: a comment")).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid(@"msgid ""The msgid text""")).Returns(true);
            A.CallTo(() => _lineValidationSessionMock.IsValid("somethingwrong")).Returns(false);

            // Act
            testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            A.CallTo(() => _messageReporterMock.ReportMessage(
                testee,
                TestFilePath,
                ErrorLevel.Error,
                A<string>.Ignored,
                "4: somethingwrong"
                )).MustHaveHappened();
        }

        [Test]
        public void Sniff_ShouldIgnoreEmptyLines()
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
            A.CallTo(() => _lineValidationSessionMock.IsValid(A<string>.Ignored)).Returns(true);

            // Act
            testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            A.CallTo(() => _lineValidationSessionMock.IsValid(string.Empty)).MustNotHaveHappened();
        }

        private PoFileSniffer CreateTestee(string testString)
        {
            var fileHelperMock = A.Fake<IFileHelper>();

            var lines = (testString + Environment.NewLine + MarkerLines.EndOfFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var extendedStreamReaderMock = A.Fake<IExtendedStreamReader>();
            A.CallTo(() => extendedStreamReaderMock.GetLinesWithEofLine()).Returns(lines);
            A.CallTo(() => fileHelperMock.GetExtendedStreamReader(TestFilePath)).Returns(extendedStreamReaderMock);

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineValidationSession()).Returns(_lineValidationSessionMock);

            return new PoFileSniffer(fileHelperMock, lineParserMock);
        }
    }
}
