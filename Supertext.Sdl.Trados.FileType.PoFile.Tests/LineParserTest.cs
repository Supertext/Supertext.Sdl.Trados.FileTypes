using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class LineParserTest
    {
        [Test]
        public void IsValid_WhenStartIsFollowedByMsgId_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenStartIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenStartIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenStartIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(string.Empty);
         
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenStartIsFollowedByCommentAndThenByEnd_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenStartIsFollowedByMsgidAndThenTextAndThenByEnd_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenMsgctxtIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidIsFollowedByMsgstr_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");

            // Act
            var result = lineValidationSession.IsValid(@"""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidIsFollowedByMsgidplural_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidIsFollowedByTextAndThenByMsgid_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");

            // Act
            var result = lineValidationSession.IsValid(@"""The msgstr text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrIsFollowedByEndOfFile_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidpluralIsFollowedByMsgstrplural_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgidpluralIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural """"");

            // Act
            var result = lineValidationSession.IsValid(@"""The text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByMsgstrplural_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr[1] ""The msgstr[1] text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"""The text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMsgstrpluralIsFollowedByEndOfFile_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"""More text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidIsFollowedByMsgstr_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidIsFollowedByMsgidpluralAndThenByTextAndThenByMsgstr_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural """"");
            lineValidationSession.IsValid(@"""The msgid_plural text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(@"""More text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrIsFollowedByEndOfFile_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr """"");
            lineValidationSession.IsValid(@"""The msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidpluralIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural """"");
            lineValidationSession.IsValid(@"""The msgid_plural text""");

            // Act
            var result = lineValidationSession.IsValid(@"""The text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidpluralIsFollowedByMsgstrplural_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByMsgstrplural_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr[1] ""The msgstr[1] text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByText_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"""The text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgstrpluralIsFollowedByEndOfFile_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");
            lineValidationSession.IsValid(@"msgstr[0] ""The msgstr[0] text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextAfterMsgidpluralIsFollowedByEndOfFile_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgid_plural ""The msgid_plural text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenCommentIsFollowedByComment_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");

            // Act
            var result = lineValidationSession.IsValid(@"#, another comment");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenCommentIsFollowedByMsgctxt_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");

            // Act
            var result = lineValidationSession.IsValid(@"msgctxt ""The msgctxt text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenCommentIsFollowedByMsgid_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenCommentIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMultipleLinesInCorrectOrder_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");
            result &= lineValidationSession.IsValid(@"#, a following comment");
            result &= lineValidationSession.IsValid(@"msgid ""The msgid text""");
            result &= lineValidationSession.IsValid(@"msgstr ""The msgstr text""");
            result &= lineValidationSession.IsValid(@"#: a comment");
            result &= lineValidationSession.IsValid(@"msgid ""The second msgid text""");
            result &= lineValidationSession.IsValid(@"msgstr ""The second msgstr text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenMultipleLinesInIncorrectOrder_ShouldReturnFalseAtFaultyLine()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");
            lineValidationSession.IsValid(@"#, a following comment");
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenMsgstrIsMissingBetweenMsgids_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgid ""The second msgid text""");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenMsgstrIsMissingUntilEnd_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenReachedEndRightAfterValidationStart_ShoudReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_WhenAllIsValidUntilFileEnd_ShoudReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"#: a comment");
            lineValidationSession.IsValid(@"#, a following comment");
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");
            lineValidationSession.IsValid(@"#: a comment");
            lineValidationSession.IsValid(@"msgid ""The second msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The second msgstr text""");

            // Act
            var result = lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenHasEmptyLines_ShoudIgnoreAndReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();


            // Act
            var result = lineValidationSession.IsValid(@"#: a comment");
            result &= lineValidationSession.IsValid(@"#, a following comment");
            result &= lineValidationSession.IsValid(@"msgid ""The msgid text""");
            result &= lineValidationSession.IsValid(@"msgstr ""The msgstr text""");
            result &= lineValidationSession.IsValid(string.Empty);
            result &= lineValidationSession.IsValid(@"msgid ""The second msgid text""");
            result &= lineValidationSession.IsValid(@"msgstr ""The second msgstr text""");
            result &= lineValidationSession.IsValid(MarkerLines.EndOfFile);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenHasMultipleTextAndComments_ShoudIgnoreAndReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsValid(@"msgid """"");
            result &= lineValidationSession.IsValid(@"msgstr """"");
            result &= lineValidationSession.IsValid(@"""Project - Id - Version: supertext2014\n""");
            result &= lineValidationSession.IsValid(@"""POT - Creation - Date: 2016 - 02 - 01 14:23 + 0100\n""");
            result &= lineValidationSession.IsValid(@"""X-Poedit-SearchPath-0: /var/www/blog/wp-content/themes/supertext2014\n""");
            result &= lineValidationSession.IsValid(string.Empty);
            result &= lineValidationSession.IsValid(@"#: /var/www/blog/wp-content/themes/supertext2014/content-gallery.php:39");
            result &= lineValidationSession.IsValid(@"#: /var/www/blog/wp-content/themes/supertext2014/content.php:120");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void NextExpectedLineDescription_ShouldReturnPattern()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.NextExpectedLineDescription;

            // Assert
            result.Should().NotBeEmpty();
        }

        [Test]
        public void Parse_WhenMsgctxtLine_ShouldReturnMessageContextAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();

            // Act
            var result = lineParsingSession.Parse(@"msgctxt ""The msgctxt text""");

            // Assert
            result.LineType.Should().Be(LineType.MessageContext);
            result.LineContent.Should().Be("The msgctxt text");
        }

        [Test]
        public void Parse_WhenMsgidLine_ShouldReturnMessageIdTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();

            // Act
            var result = lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Assert
            result.LineType.Should().Be(LineType.MessageId);
            result.LineContent.Should().Be("The msgid text");
        }

        [Test]
        public void Parse_WhenMsgstrLine_ShouldReturnMessageStringTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Act
            var result = lineParsingSession.Parse(@"msgstr ""The msgstr text""");

            // Assert
            result.LineType.Should().Be(LineType.MessageString);
            result.LineContent.Should().Be("The msgstr text");
        }

        [Test]
        public void Parse_WhenMsgidpluralLine_ShouldReturnMessageIdPluralTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Act
            var result = lineParsingSession.Parse(@"msgid_plural ""The msgid_plural text""");

            // Assert
            result.LineType.Should().Be(LineType.MessageIdPlural);
            result.LineContent.Should().Be("The msgid_plural text");
        }

        [Test]
        public void Parse_WhenMsgstrpluralLine_ShouldReturnMessageStringPluralTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");
            lineParsingSession.Parse(@"msgid_plural ""The msgid_plural text""");

            // Act
            var result = lineParsingSession.Parse(@"msgstr[0] ""The msgstr[0] text""");

            // Assert
            result.LineType.Should().Be(LineType.MessageStringPlural);
            result.LineContent.Should().Be("The msgstr[0] text");
        }

        [Test]
        public void Parse_WhenTextLine_ShouldReturnTextTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid """"");

            // Act
            var result = lineParsingSession.Parse(@"""The text""");

            // Assert
            result.LineType.Should().Be(LineType.Text);
            result.LineContent.Should().Be("The text");
        }

        [Test]
        public void Parse_WhenCommentLine_ShouldReturnCommentTypeAndContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();

            // Act
            var result = lineParsingSession.Parse(@"#: some comment");

            // Assert
            result.LineType.Should().Be(LineType.Comment);
            result.LineContent.Should().Be(": some comment");
        }

        [Test]
        public void Parse_WhenEmptyLine_ShouldReturnEmptyLineTypeAndEmptyContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();

            // Act
            var result = lineParsingSession.Parse(string.Empty);

            // Assert
            result.LineType.Should().Be(LineType.Empty);
            result.LineContent.Should().Be(string.Empty);
        }

        [Test]
        public void Parse_WhenEndOfFile_ShouldReturnEndOFileTypeAndEmptyContent()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");
            lineParsingSession.Parse(@"msgstr ""The msgstr text""");

            // Act
            var result = lineParsingSession.Parse(MarkerLines.EndOfFile);

            // Assert
            result.LineType.Should().Be(LineType.EndOfFile);
            result.LineContent.Should().Be(string.Empty);
        }

        [Test]
        public void Parse_WhenMsgid_ShouldAddMsgidLineKeyword()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            
            // Act
            var result = lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Assert
            result.LineKeyword.Should().Be("msgid");
        }


        [Test]
        public void Parse_WhenMsgidPlural_ShouldAddMsgidLineKeyword()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Act
            var result = lineParsingSession.Parse(@"msgid_plural ""The msgid_plural text""");

            // Assert
            result.LineKeyword.Should().Be("msgid_plural");
        }

        [Test]
        public void Parse_WhenMsgstr_ShouldAddMsgstrLineKeyword()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");

            // Act
            var result = lineParsingSession.Parse(@"msgstr ""The msgstr text""");

            // Assert
            result.LineKeyword.Should().Be("msgstr");
        }

        [Test]
        public void Parse_WhenMsgstrPlural_ShouldAddMsgstrPluralLineKeyword()
        {
            // Arrange
            var testee = new LineParser();
            var lineParsingSession = testee.StartLineParsingSession();
            lineParsingSession.Parse(@"msgid ""The msgid text""");
            lineParsingSession.Parse(@"msgid_plural ""The msgid_plural text""");

            // Act
            var result = lineParsingSession.Parse(@"msgstr[0] ""The msgstr text""");

            // Assert
            result.LineKeyword.Should().Be("msgstr[0]");
        }
    }
}