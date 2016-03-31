using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class EntryBuilderTest
    {
        [Test]
        public void Add_WhenNotAddedAllEntryLines_ShouldCompleteEntryBeNull()
        {
            // Arrange
            var testee = new EntryBuilder();
            // Act
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);

            // Assert
            testee.CompleteEntry.Should().BeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingOtherLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.EndOfFile, string.Empty, MarkerLines.EndOfFile), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingEmptyLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingNextEntryLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenMsgidIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The msgid text");
        }

        [Test]
        public void Add_WhenMsgidIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "", "msgid"), 1);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The textThe second text");
        }

        [Test]
        public void Add_WhenMsgidIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The msgid textThe textThe second text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The msgstr text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "", "msgstr"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The textThe second text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The msgstr textThe textThe second text");
        }

        [Test]
        public void Add_WhenMsgidOnOneLine_ShouldSetMsgidLocationToOneLine()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageIdStart.Should().Be(1);
            testee.CompleteEntry.MessageIdEnd.Should().Be(1);
        }

        [Test]
        public void Add_WhenMsgidOnMultipleLines_ShouldSetMsgidLocationToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "", "msgid"), 1);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageIdStart.Should().Be(1);
            testee.CompleteEntry.MessageIdEnd.Should().Be(3);
        }

        [Test]
        public void Add_WhenMsgstrOnOneLine_ShouldSetMsgstrLocationToOneLine()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageStringStart.Should().Be(2);
            testee.CompleteEntry.MessageStringEnd.Should().Be(2);
        }

        [Test]
        public void Add_WhenMsgstrOnMultipleLines_ShouldSetMsgstrContextToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "", "msgstr"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageStringStart.Should().Be(2);
            testee.CompleteEntry.MessageStringEnd.Should().Be(4);
        }

        [Test]
        public void Add_WhenEntryHasContext_ShouldCreateEntryWithContext()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageContext, "The msgctxt text", "msgctxt"), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.MessageContext.Should().Be("The msgctxt text");
        }

        [Test]
        public void Add_WhenEntryHasMsgidPlural_ShouldCreateEntryWithMsgidPlural()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text", "msgid_plural"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text", "msgstr[0]"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.MessageIdPlural.Should().Be("The msgid_plural text");
        }

        [Test]
        public void Add_WhenEntryHasMsgidPluralOnMultipleLines_ShouldCreateEntryWithMsgidPluralAndAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text", "msgid_plural"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text", "\""), 4);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text", "msgstr[0]"), 5);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageIdPlural.Should().Be("The msgid_plural textThe textThe second text");
        }

        [Test]
        public void Add_WhenEntryHasMsgstrPlural_ShouldCreateEntryWithMsgstrPlural()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text", "msgid_plural"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text", "msgstr[0]"), 3);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text", "msgstr[0]"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageStringPlural[0].Should().Be("The msgstr[0] text");
            testee.CompleteEntry.MessageStringPlural[1].Should().Be("The msgstr[1] text");
        }

        [Test]
        public void Add_WhenEntryHasMsgstrPluralOnMultipleLines_ShouldCreateEntryWithMsgstrPluralWithAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text", "msgid_plural"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text", "msgstr[0]"), 3);
            testee.Add(new ParseResult(LineType.Text, "The text", "\""), 4);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text", "msgstr[0]"), 5);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageStringPlural[0].Should().Be("The msgstr[0] textThe text");
        }

        [Test]
        public void Add_WhenEntryHasComment_ShouldCreateEntryWithComment()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Comment, ", the comment text", "#"), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.Comments[0].Should().Be(", the comment text");
        }

        [Test]
        public void Add_WhenEntryHasComments_ShouldCreateEntryWithComments()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Comment, ", the comment text", "#"), 1);
            testee.Add(new ParseResult(LineType.Comment, ", the second comment text", "#"), 2);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text", "msgid"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text", "msgstr"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.Comments[0].Should().Be(", the comment text");
            testee.CompleteEntry.Comments[1].Should().Be(", the second comment text");
        }
    }
}
