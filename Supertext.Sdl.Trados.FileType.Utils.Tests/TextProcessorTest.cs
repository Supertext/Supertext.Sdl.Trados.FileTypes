using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.Utils.Tests
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
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasPercentWithNumberPlaceholder_ShouldRecognizePlaceholder()
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
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasStartTagButNoEndTag_ShouldReturnAsPlaceholder()
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
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasEndTagButNoStartTag_ShouldReturnAsPlaceholder()
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
        public void Process_WhenUsingDefaultEmbeddedPatternsAndTextHasIncompleteTags_ShouldReturnAllAsPlaceholder()
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
        public void Process_WhenTagContainsOtherTags_ShouldReturnOutterTagOnly()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(EmbeddedContentRegexSettings.DefaultMatchRules.ToList());
            var testString = @"<a href=""%s"">log in</a>";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.StartTag);
            result[1].InlineType.Should().Be(InlineType.Text);
            result[2].InlineType.Should().Be(InlineType.EndTag);
        }

        [Test]
        public void Process_WhenTagsOverlapp_ShouldReturnBiggerTagAsTagOnly()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = "##",
                    TagType = MatchRule.TagTypeOption.Placeholder
                },
                new MatchRule
                {
                    StartTagRegexValue = @"##\>",
                    EndTagRegexValue = @"\<##",
                    TagType = MatchRule.TagTypeOption.TagPair
                }
            });
            var testString = @"##>This<## is a test";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.StartTag);
            result[1].InlineType.Should().Be(InlineType.Text);
            result[2].InlineType.Should().Be(InlineType.EndTag);
            result[3].InlineType.Should().Be(InlineType.Text);
        }

        [Test]
        public void Process_WhenTextHasTagsAndOneTagIsAlsoPlaceholder_ShouldReturnTagsAsTag()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"<br\s*>",
                    TagType = MatchRule.TagTypeOption.Placeholder
                },
                new MatchRule
                {
                    TagType = MatchRule.TagTypeOption.TagPair,
                    StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*([^<>\/]|\"".*\"")*>",
                    EndTagRegexValue = @"<\/[a-zA-Z][a-zA-Z0-9]*[^<>]*>",
                    SegmentationHint = SegmentationHint.IncludeWithText,
                    IsContentTranslatable = true
                }
            });
            var testString = @"<span>This <br> is a test</span>";

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

        [Test]
        public void Process_ShouldReturnFragmentsWithSegmentationHint()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"\[\[",
                    EndTagRegexValue = @"\]\]",
                    TagType = MatchRule.TagTypeOption.TagPair,
                    SegmentationHint = SegmentationHint.IncludeWithText
                }
            });
            var testString = @"Some text with [[more text]].";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[1].SegmentationHint.Should().Be(SegmentationHint.IncludeWithText);
            result[3].SegmentationHint.Should().Be(SegmentationHint.IncludeWithText);
        }

        [Test]
        public void Process_WhenIsContentTranslatableIsFalse_ShouldReturnFragmentsWithIsContentTranslatableFalse()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"\[\[",
                    EndTagRegexValue = @"\]\]",
                    TagType = MatchRule.TagTypeOption.TagPair,
                    SegmentationHint = SegmentationHint.IncludeWithText,
                    IsContentTranslatable = false
                }
            });
            var testString = @"Some text with [[more text]].";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[1].IsContentTranslatable.Should().BeFalse();
            result[3].IsContentTranslatable.Should().BeFalse();
        }

        [Test]
        public void Process_WhenIsContentTranslatableIsTrue_ShouldReturnFragmentsWithIsContentTranslatableTrue()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = @"\[\[",
                    EndTagRegexValue = @"\]\]",
                    TagType = MatchRule.TagTypeOption.TagPair,
                    SegmentationHint = SegmentationHint.IncludeWithText,
                    IsContentTranslatable = true
                }
            });
            var testString = @"Some text with [[more text]].";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].IsContentTranslatable.Should().BeTrue();
        }

        [Test]
        public void Process_WhenIgnoreCaseIsTrue_ShouldIgnoreCases()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = "abc",
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    IgnoreCase = true
                }
            });
            var testString = @"ABC";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Placeholder);
        }

        [Test]
        public void Process_WhenIgnoreCaseIsFalse_ShouldNotIgnoreCases()
        {
            // Arrange
            var testee = new TextProcessor();
            testee.InitializeWith(new List<MatchRule>
            {
                new MatchRule
                {
                    StartTagRegexValue = "abc",
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    IgnoreCase = false
                }
            });
            var testString = @"ABC";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Text);
        }
    }
}