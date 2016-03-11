using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class LineParserTest
    {
        [Test]
        public void Check_WhenStartIsFollowedByMsgId_ShouldReturnTrue()
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
        public void Check_WhenStartIsFollowedByComment_ShouldReturnTrue()
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
        public void Check_WhenMsgidIsFollowedByMsgstr_ShouldReturnTrue()
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
        public void Check_WhenMsgidIsFollowedByText_ShouldReturnTrue()
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
        public void Check_WhenMsgstrIsFollowedByText_ShouldReturnTrue()
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
        public void Check_WhenMsgstrIsFollowedByComment_ShouldReturnTrue()
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
        public void Check_WhenMsgstrIsFollowedByMsgid_ShouldReturnTrue()
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
        public void Check_WhenTextIsFollowedByText_ShouldReturnTrue()
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
        public void Check_WhenTextIsFollowedByComment_ShouldReturnTrue()
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
        public void Check_WhenTextIsFollowedByMsgstr_ShouldReturnTrue()
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
        public void Check_WhenTextIsFollowedByMsgid_ShouldReturnTrue()
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
        public void Check_WhenCommentIsFollowedByComment_ShouldReturnTrue()
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
        public void Check_WhenCommentIsFollowedByMsgid_ShouldReturnTrue()
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
        public void Check_WhenMultipleLinesInCorrectOrder_ShouldAlwaysReturnTrue()
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
        public void Check_WhenMultipleLinesInIncorrectOrder_ShouldReturnFalseAtFaultyLine()
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
        public void Check_WhenMsgstrIsMissingBetweenMsgids_ShouldReturnFalse()
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
        public void IsEndValid_WhenMsgstrIsMissingUntilEnd_ShouldReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();
            lineValidationSession.IsValid(@"msgid ""The msgid text""");

            // Act
            var result = lineValidationSession.IsEndValid();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsEndValid_WhenReachedEndRightAfterValidationStart_ShoudReturnFalse()
        {
            // Arrange
            var testee = new LineParser();
            var lineValidationSession = testee.StartLineValidationSession();

            // Act
            var result = lineValidationSession.IsEndValid();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsEndValid_WhenAllIsValid_ShoudReturnTrue()
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
            var result = lineValidationSession.IsEndValid();

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


    }
}