using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class TextProcessorTest
    {
        [Test]
        public void Process_WhenUsingCustomRegex_ShouldRecognizeRegexAndReturnCorrectType()
        {
            // Arrange
            var testee = new TextProcessor(new Dictionary<string, InlineType>
            {
                {@"{\d}", InlineType.Placeholder }
            });

            var testString = @"{4}";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
            result[0].Content.Should().Be("{4}");
        }

        [Test]
        public void Process_WhenTextHasEmbeddedContentAtStart_ShouldReturnTextAfterEmbeddedContent()
        {
            // Arrange
            var testee = new TextProcessor(new Dictionary<string, InlineType>
            {
                {@"{\d}", InlineType.Placeholder }
            });

            var testString = @"{4} this is some text";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[1].InlineType.Should().Be(InlineType.Text);
            result[1].Content.Should().Be(" this is some text");
        }

        [Test]
        public void Process_WhenTextHasEmbeddedContentInMiddle_ShouldReturnTextBeforeAndAfterEmbeddedContent()
        {
            // Arrange
            var testee = new TextProcessor(new Dictionary<string, InlineType>
            {
                {@"{\d}", InlineType.Placeholder }
            });

            var testString = @"This is the start {4} and this is the end.";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Text);
            result[0].Content.Should().Be("This is the start ");
            result[2].InlineType.Should().Be(InlineType.Text);
            result[2].Content.Should().Be(" and this is the end.");
        }

        [Test]
        public void Process_WhenTextHasEmbeddedContentAtEnd_ShouldReturnTextBeforeEmbeddedContent()
        {
            // Arrange
            var testee = new TextProcessor(new Dictionary<string, InlineType>
            {
                {@"{\d}", InlineType.Placeholder }
            });

            var testString = @"This is the start {4}";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Text);
            result[0].Content.Should().Be("This is the start ");
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedRegexsAndTextHasPercentPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor(TextProcessor.DefaultEmbeddedContentRegexs);
            var testString = @"%123";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedRegexsAndTextHasPercentWithWordPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor(TextProcessor.DefaultEmbeddedContentRegexs);
            var testString = @"%s";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedRegexsAndTextHasDollarPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor(TextProcessor.DefaultEmbeddedContentRegexs);
            var testString = @"$test";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedRegexsAndTextHasStartTag_ShouldRecognizeStartTag()
        {
            // Arrange
            var testee = new TextProcessor(TextProcessor.DefaultEmbeddedContentRegexs);
            var testString = @"<a href=""http://www.supertext.ch"">";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.StartTag);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedRegexsAndTextHasEndTag_ShouldRecognizeEndTag()
        {
            // Arrange
            var testee = new TextProcessor(TextProcessor.DefaultEmbeddedContentRegexs);
            var testString = @"</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.EndTag);
        }
    }

}
