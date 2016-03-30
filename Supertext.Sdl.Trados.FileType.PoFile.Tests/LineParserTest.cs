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
        public void IsValid_WhenTextIsFollowedByText_ShouldReturnTrue()
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
        public void IsValid_WhenTextIsFollowedByComment_ShouldReturnTrue()
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
        public void IsValid_WhenTextIsFollowedByMsgstr_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_WhenTextIsFollowedByMsgid_ShouldReturnTrue()
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
        public void IsValid_WhenTextIsFollowedByMsgctxt_ShouldReturnTrue()
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
        public void IsValid_WhenTextIsFollowedByEmptyLine_ShouldReturnTrue()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid """"");
            lineValidationSession.IsValid(@"""The text""");

            // Act
            var result = lineValidationSession.IsValid(string.Empty);

            // Assert
            result.Should().BeTrue();
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
        public void NextExpectedLineDescription_WhenMandortyLinePatternExpected_ShouldReturnPattern()
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
        public void NextExpectedLineDescription_WhenNoMandortyLinePatternExpected_ShouldReturnEmptyString()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");
            lineValidationSession.IsValid(@"msgstr ""The msgstr text""");

            // Act
            var result = lineValidationSession.NextExpectedLineDescription;

            // Assert
            result.Should().BeEmpty();
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
            result.LineContent.Should().Be("some comment");
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
    }
}