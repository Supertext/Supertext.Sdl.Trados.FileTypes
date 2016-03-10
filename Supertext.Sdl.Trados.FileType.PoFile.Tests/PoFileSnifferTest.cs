using System.IO;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

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
        private ILineValidationSession _lineValidationSession;


        [SetUp]
        public void SetUp()
        {
            _testLanguage = new Language();
            _testCodepage = new Codepage();
            _messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            _settingsGroupMock = A.Fake<ISettingsGroup>();
            _lineValidationSession = A.Fake<ILineValidationSession>();
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
            A.CallTo(() => _lineValidationSession.Check(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSession.IsEndValid()).Returns(true);

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
            A.CallTo(() => _lineValidationSession.Check(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSession.IsEndValid()).Returns(false);

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
            A.CallTo(() => _lineValidationSession.Check(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _lineValidationSession.IsEndValid()).Returns(false);
            A.CallTo(() => _lineValidationSession.NextMandatoryLinePattern).Returns("msgstr");

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
            A.CallTo(() => _lineValidationSession.Check("#: a comment")).Returns(true);
            A.CallTo(() => _lineValidationSession.Check(@"msgid ""The msgid text""")).Returns(true);
            A.CallTo(() => _lineValidationSession.Check("somethingwrong")).Returns(false);

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
            A.CallTo(() => _lineValidationSession.Check("#: a comment")).Returns(true);
            A.CallTo(() => _lineValidationSession.Check(@"msgid ""The msgid text""")).Returns(true);
            A.CallTo(() => _lineValidationSession.Check("somethingwrong")).Returns(false);

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
            A.CallTo(() => _lineValidationSession.Check(A<string>.Ignored)).Returns(true);

            // Act
            testee.Sniff(TestFilePath, _testLanguage, _testCodepage, _messageReporterMock, _settingsGroupMock);

            // Assert
            A.CallTo(() => _lineValidationSession.Check(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(5));
        }

        private PoFileSniffer CreateTestee(string testString)
        {
            var dotNetFactoryMock = A.Fake<IDotNetFactory>();

            var lineParserMock = A.Fake<ILineParser>();
            A.CallTo(() => lineParserMock.StartLineValidationSession()).Returns(_lineValidationSession);

            var streamReaderFake = new StringReaderWrapper(testString);
            A.CallTo(() => dotNetFactoryMock.CreateStreamReader(TestFilePath)).Returns(streamReaderFake);

            return new PoFileSniffer(dotNetFactoryMock, lineParserMock);
        }

        private class StringReaderWrapper : IReader
        {
            private readonly StringReader _stringReader;

            public StringReaderWrapper(string text)
            {
                _stringReader = new StringReader(text);
            }

            public void Dispose()
            {
                _stringReader.Dispose();
            }

            public string ReadLine()
            {
                return _stringReader.ReadLine();
            }

            public void Close()
            {
                _stringReader.Close();
            }
        }
    }
}
