using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class TextProcessorTest
    {
        [Test]
        public void Process_WhenTextHasPlaceholder_ShouldRecognizePlaceholder()
        {
            // Arrange
            var embeddedContentRegexs = new Dictionary<string, InlineType>
            {
                { @"%\d+", InlineType.Placeholder }
            };

            var testee = new TextProcessor(embeddedContentRegexs);
            var testString = @"some text with %123 and more text";

            // Act
            var result = testee.Process(testString);

            // Assert
            result[0].InlineType.Should().Be(InlineType.Text);
            result[1].InlineType.Should().Be(InlineType.Placeholder);
            result[2].InlineType.Should().Be(InlineType.Text);
        }
    }

}
