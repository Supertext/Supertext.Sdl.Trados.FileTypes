using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;

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
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);

            // Assert
            testee.CompleteEntry.Should().BeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingOtherLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.EndOfFile, string.Empty), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingEmptyLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenAddedAllEntryLinesAndAddingNextEntryLine_ShouldSetCompleteEntry()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 3);

            // Assert
            testee.CompleteEntry.Should().NotBeNull();
        }

        [Test]
        public void Add_WhenOneEntryAfterTheOther_ShouldNotHaveEntryDataFromPrevious()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageContext, "The msgctxt text"), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.MessageId, "The second msgid text"), 4);
            testee.Add(new ParseResult(LineType.MessageString, "The second msgstr text"), 5);
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageContext.Should().BeEmpty();
        }

        [Test]
        public void Add_WhenEntryStartsWithComment_ShouldSetCorrectEntryLocation()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 1);
            testee.Add(new ParseResult(LineType.Comment, "# some comment"), 2);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.Start.Should().Be(2);
            testee.CompleteEntry.End.Should().Be(4);
        }

        public void Add_WhenEntryStartsWithMsgctxt_ShouldSetCorrectEntryLocation()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 1);
            testee.Add(new ParseResult(LineType.MessageContext, "The msgctxt text"), 2);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.Start.Should().Be(2);
            testee.CompleteEntry.End.Should().Be(4);
        }

        [Test]
        public void Add_WhenEntryStartsWithMsgid_ShouldSetCorrectEntryLocation()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.Start.Should().Be(2);
            testee.CompleteEntry.End.Should().Be(3);
        }

        [Test]
        public void Add_WhenMsgidIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The msgid text");
        }

        [Test]
        public void Add_WhenMsgidIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, ""), 1);
            testee.Add(new ParseResult(LineType.Text, "The text"), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The textThe second text");
        }

        [Test]
        public void Add_WhenMsgidIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.Text, "The text"), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageId.Should().Be("The msgid textThe textThe second text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnOneLine_ShouldTakeText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The msgstr text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnMultipleLinesStartingWithEmptyString_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, ""), 2);
            testee.Add(new ParseResult(LineType.Text, "The text"), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The textThe second text");
        }

        [Test]
        public void Add_WhenMsgstrIsOnMultipleLines_ShouldTakeAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text"), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageString.Should().Be("The msgstr textThe textThe second text");
        }

        [Test]
        public void Add_WhenMsgidOnOneLine_ShouldSetMsgidLocationToOneLine()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageIdStart.Should().Be(1);
            testee.CompleteEntry.MessageIdEnd.Should().Be(1);
        }

        [Test]
        public void Add_WhenMsgidOnMultipleLines_ShouldSetMsgidLocationToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, ""), 1);
            testee.Add(new ParseResult(LineType.Text, "The text"), 2);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageIdStart.Should().Be(1);
            testee.CompleteEntry.MessageIdEnd.Should().Be(3);
        }

        [Test]
        public void Add_WhenMsgidHasPlural_ShouldSetMsgidLocationToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.MessageIdStart.Should().Be(1);
            testee.CompleteEntry.MessageIdEnd.Should().Be(2);
        }

        [Test]
        public void Add_WhenMsgstrOnOneLine_ShouldSetMsgstrLocationToOneLine()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 2);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 3);

            // Assert
            testee.CompleteEntry.MessageStringStart.Should().Be(2);
            testee.CompleteEntry.MessageStringEnd.Should().Be(2);
        }

        [Test]
        public void Add_WhenMsgstrOnMultipleLines_ShouldSetMsgstrContextToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageString, ""), 2);
            testee.Add(new ParseResult(LineType.Text, "The text"), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageStringStart.Should().Be(2);
            testee.CompleteEntry.MessageStringEnd.Should().Be(4);
        }

        [Test]
        public void Add_WhenMsgstrPlurals_ShouldSetMsgstrContextToAllTheLines()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text"), 3);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text"), 4);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[2] text"), 5);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageStringStart.Should().Be(3);
            testee.CompleteEntry.MessageStringEnd.Should().Be(5);
        }

        [Test]
        public void Add_WhenEntryHasContext_ShouldCreateEntryWithContext()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageContext, "The msgctxt text"), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.MessageContext.Should().Be("The msgctxt text");
        }

        [Test]
        public void Add_WhenEntryHasMsgidPlural_ShouldCreateEntryWithMsgidPlural()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.MessageIdPlural.Should().Be("The msgid_plural text");
        }

        [Test]
        public void Add_WhenEntryHasMsgidPluralOnMultipleLines_ShouldCreateEntryWithMsgidPluralAndAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.Text, "The text"), 3);
            testee.Add(new ParseResult(LineType.Text, "The second text"), 4);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text"), 5);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageIdPlural.Should().Be("The msgid_plural textThe textThe second text");
        }

        [Test]
        public void Add_WhenEntryHasMsgstrPlural_ShouldCreateEntryWithMsgstrPlural()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text"), 3);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.MessageStringPlurals[0].Should().Be("The msgstr[0] text");
            testee.CompleteEntry.MessageStringPlurals[1].Should().Be("The msgstr[1] text");
        }

        [Test]
        public void Add_WhenEntryHasMsgstrPluralOnMultipleLines_ShouldCreateEntryWithMsgstrPluralWithAllText()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 1);
            testee.Add(new ParseResult(LineType.MessageIdPlural, "The msgid_plural text"), 2);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[0] text"), 3);
            testee.Add(new ParseResult(LineType.Text, "The text"), 4);
            testee.Add(new ParseResult(LineType.MessageStringPlural, "The msgstr[1] text"), 5);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 6);

            // Assert
            testee.CompleteEntry.MessageStringPlurals[0].Should().Be("The msgstr[0] textThe text");
        }

        [Test]
        public void Add_WhenEntryHasComment_ShouldCreateEntryWithComment()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Comment, ", the comment text"), 1);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 2);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 3);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 4);

            // Assert
            testee.CompleteEntry.Comments[0].Should().Be(", the comment text");
        }

        [Test]
        public void Add_WhenEntryHasComments_ShouldCreateEntryWithComments()
        {
            // Arrange
            var testee = new EntryBuilder();
            testee.Add(new ParseResult(LineType.Comment, ", the comment text"), 1);
            testee.Add(new ParseResult(LineType.Comment, ", the second comment text"), 2);
            testee.Add(new ParseResult(LineType.MessageId, "The msgid text"), 3);
            testee.Add(new ParseResult(LineType.MessageString, "The msgstr text"), 4);

            // Act
            testee.Add(new ParseResult(LineType.Empty, string.Empty), 5);

            // Assert
            testee.CompleteEntry.Comments[0].Should().Be(", the comment text");
            testee.CompleteEntry.Comments[1].Should().Be(", the second comment text");
        }


    }
}
