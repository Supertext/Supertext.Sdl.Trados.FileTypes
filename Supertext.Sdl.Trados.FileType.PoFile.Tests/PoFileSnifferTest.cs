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

        [Test]
        public void Sniff_WhenFileFormatIsOk_ShouldReturnIsSupported()
        {
            // Arrange
            var testString = @"
#: /var/www/blog/wp-content/themes/supertext2014/tag.php:21
#, php-format
msgid ""Schlagwort-Archiv: %s""
msgstr ""Archives par mot-clé: %s""

#: /var/www/blog/wp-content/themes/supertext2014/sidebar.php:1
msgid ""Supersozial""
msgstr ""Supersocial""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeTrue();
        }

        [Test]
        public void Sniff_WhenCommentIsWrongPlace_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid ""Schlagwort-Archiv: %s""
#: /var/www/blog/wp-content/themes/supertext2014/tag.php:21
msgstr ""Archives par mot-clé: %s""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenMsgstrIsBeforeMsgid_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid ""Supersozial""
msgstr ""Supersocial""

msgstr ""Archives par mot-clé: %s""
msgid ""Schlagwort-Archiv: %s""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenMsgidIsFollowedByTextAndEndOfFile_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid """"
""Supersozial""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenMsgidIsFollowedByTextButNoMsgstr_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
#: /var/www/blog/wp-content/themes/supertext2014/tag.php:21
#, php-format
msgid ""Schlagwort-Archiv: %s""

#: /var/www/blog/wp-content/themes/supertext2014/sidebar.php:1
msgid ""Supersozial""
msgstr ""Supersocial""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }


        [Test]
        public void Sniff_WhenLineHasUnexpectedStart_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid ""Supersozial""
msgstr ""Supersocial""

thisiswrong
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenMsgidIsNotFollowedByText_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid thisiswrong
msgstr ""Supersocial""
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenMsgstrIsNotFollowedByText_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
msgid ""Supersozial""
msgstr thisiswrong
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenNoMsgidAndMsgstrExists_ShouldReturnIsNotSupported()
        {
            // Arrange
            var testString = @"
#: /var/www/blog/wp-content/themes/supertext2014/tag.php:21
#, php-format
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            var result = testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            result.IsSupported.Should().BeFalse();
        }

        [Test]
        public void Sniff_WhenFormatWrong_ShouldReportWhichLineHasError()
        {
            // Arrange
            var testString = @"
msgid ""Supersozial""
msgstr ""Supersocial""

thisiswrong
";

            var testee = CreateTestee(testString);
            var language = new Language();
            var codepage = new Codepage();
            var messageReporterMock = A.Fake<INativeTextLocationMessageReporter>();
            var settingsGroupMock = A.Fake<ISettingsGroup>();

            // Act
            testee.Sniff(TestFilePath, language, codepage, messageReporterMock, settingsGroupMock);

            // Assert
            A.CallTo(() => messageReporterMock.ReportMessage(
                testee,
                TestFilePath,
                ErrorLevel.Error, 
                A<string>.Ignored,
                "5: thisiswrong"
                )).MustHaveHappened();
        }

        private static PoFileSniffer CreateTestee(string testString)
        {
            var dotNetFactoryMock = A.Fake<IDotNetFactory>();
            var streamReaderFake = new StringReaderWrapper(testString);
            A.CallTo(() => dotNetFactoryMock.CreateStreamReader(TestFilePath)).Returns(streamReaderFake);
            var testee = new PoFileSniffer(dotNetFactoryMock, new LineValidator());
            return testee;
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
