using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class TextProcessorTest
    {
        [Test]
        public void Process_WhenUsingCustomRegex_ShouldRecognizeRegexAndReturnCorrectType()
        {
            // Arrange
            var matchRules = new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"{\d}",
                    TagType = MatchRule.TagTypeOption.Placeholder
                }
            };
            var testee = new TextProcessor();
            testee.InitializeWith(matchRules);
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
            var matchRules = new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"{\d}",
                    TagType = MatchRule.TagTypeOption.Placeholder
                }
            };
            var testee = new TextProcessor();
            testee.InitializeWith(matchRules);
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
            var matchRules = new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"{\d}",
                    TagType = MatchRule.TagTypeOption.Placeholder
                }
            };
            var testee = new TextProcessor();
            testee.InitializeWith(matchRules);
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
            var matchRules = new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"{\d}",
                    TagType = MatchRule.TagTypeOption.Placeholder
                }
            };
            var testee = new TextProcessor();
            testee.InitializeWith(matchRules);
            var testString = @"This is the start {4}";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Text);
            result[0].Content.Should().Be("This is the start ");
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasPercentPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"%123";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void
            Process_WhenUsingDefaultEmbeddedPatternsAndTextHasPercentWithWordPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"%s";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasDollarPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"$test";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasTags_ShouldRecognizeTags()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"<a href=""http://www.supertext.ch"">Some text</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.StartTag);
            result[2].InlineType.Should().Be(InlineType.EndTag);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasStartTagButNoEndTag_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"<a href=""http://www.supertext.ch"">";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasEndTagButNoStartTag_ShouldRecognizePlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasIncompleteTags_ShouldRecognizeAllAsPlaceholder()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"<a href=""http://www.supertext.ch"">Some text with <span>more text</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
            result[1].InlineType.Should().Be(InlineType.Text);
            result[2].InlineType.Should().Be(InlineType.Placeholder);
            result[3].InlineType.Should().Be(InlineType.Text);
            result[4].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenTextHasCompleteTagsFromOneMatchRuleAndIncompleteTagsFromOtherMatchRule_ShouldRecognizeCompleteTagsFromOneMatchRuleAsStartAndEndTag()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"\[\[",
                    EndTagRegexValue = @"\]\]",
                    TagType = MatchRule.TagTypeOption.TagPair
                },
                new MatchRule
                {
                    TagType = MatchRule.TagTypeOption.TagPair,
                    StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*[^<>]*>",
                    EndTagRegexValue = @"<\/[a-zA-Z][a-zA-Z0-9]*[^<>]*>"
                }
            });
            var testString = @"<a href=""http://www.supertext.ch"">Some text with [[more text</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.StartTag);
            result[1].InlineType.Should().Be(InlineType.Text);
            result[2].InlineType.Should().Be(InlineType.Placeholder);
            result[3].InlineType.Should().Be(InlineType.Text);
            result[4].InlineType.Should().Be(InlineType.EndTag);
        }

        [Test]
        public void Process_WhenUsingNoMatchRules_ShouldRecognizeAllAsText()
        {
            // Arrange
            var testee = new TextProcessor();
            var testString = @"This is some text.";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].Content.Should().Be("This is some text.");
            result[0].InlineType.Should().Be(InlineType.Text);
        }
    }
}