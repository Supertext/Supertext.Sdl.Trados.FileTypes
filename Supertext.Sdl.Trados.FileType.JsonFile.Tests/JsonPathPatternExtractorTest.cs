using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class JsonPathPatternExtractorTest
    {
        private static IJsonFactory _jsonFactoryMock;
        private const string Testfile = "testfile";

        [Test]
        public void ExtractPathPatterns_WhenMultipleDifferentPaths_ShouldReturnAllTheseDifferentPaths()
        {
            // Arrange
            var testee = CreateTestee(new []{ "path1", "path2", "path3" });

            // Act
            var result = testee.ExtractPathPatterns(Testfile).ToList();

            // Assert
            result.Count.Should().Be(3);
            result.Should().Contain("path1");
            result.Should().Contain("path2");
            result.Should().Contain("path3");
        }

        [Test]
        public void ExtractPathPatterns_WhenMultiplePathsWithSamePaths_ShouldReturnOnlyDifferentPaths()
        {
            // Arrange
            var testee = CreateTestee(new[] { "path1", "path1", "path5" });

            // Act
            var result = testee.ExtractPathPatterns(Testfile).ToList();

            // Assert
            result.Should().Contain("path1");
            result.Should().Contain("path5");
        }

        [Test]
        public void ExtractPathPatterns_WhenPathContainsPoint_ShouldReplacePointWithPattern()
        {
            // Arrange
            var testee = CreateTestee(new[] { "some.content" });

            // Act
            var result = testee.ExtractPathPatterns(Testfile);

            // Assert
            result.Should().Contain(@"some\.content");
        }

        [Test]
        public void ExtractPathPatterns_WhenPathContainsBrackets_ShouldReplaceBracketsWithPattern()
        {
            // Arrange
            var testee = CreateTestee(new[] { "theObject['with spaces in property name']" });

            // Act
            var result = testee.ExtractPathPatterns(Testfile);

            // Assert
            result.Should().Contain(@"theObject\['with spaces in property name'\]");
        }

        [Test]
        public void ExtractPathPatterns_WhenPathContainsArrayIndexer_ShouldReplaceIndexerWithPattern()
        {
            // Arrange
            var testee = CreateTestee(new[] { "someArray[1]" });

            // Act
            var result = testee.ExtractPathPatterns(Testfile);

            // Assert
            result.Should().Contain(@"someArray\[\d+\]");
        }

        [Test]
        public void ExtractPathPatterns_WhenTokenTypeIsNotString_ShouldNotAddPath()
        {
            // Arrange
            var testee = CreateTestee(new[] { "som path" }, JsonToken.Integer);

            // Act
            var result = testee.ExtractPathPatterns(Testfile);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void ExtractPathPatterns_WhenExceptionThrown_ShouldJustReturnEmptyList()
        {
            // Arrange
            var testee = CreateTestee(new[] { "path1", "path2" });
            A.CallTo(() => _jsonFactoryMock.CreateJsonTextReader(A<string>.Ignored)).Throws<FileNotFoundException>();

            // Act
            var result = testee.ExtractPathPatterns(Testfile);

            // Assert
            result.Should().BeEmpty();
        }

        private static JsonPathPatternExtractor CreateTestee(string[] paths, JsonToken tokenType = JsonToken.String, string value = "some value")
        {
            var jsonTextReaderMock = A.Fake<IJsonTextReader>();
            A.CallTo(() => jsonTextReaderMock.TokenType).Returns(tokenType);
            A.CallTo(() => jsonTextReaderMock.Value).Returns(value);

            var readReturns = paths.Select(path => true).ToList();
            readReturns.Add(false);

            A.CallTo(() => jsonTextReaderMock.Read()).ReturnsNextFromSequence(readReturns.ToArray());
            A.CallTo(() => jsonTextReaderMock.Path).ReturnsNextFromSequence(paths);

            _jsonFactoryMock = A.Fake<IJsonFactory>();
            A.CallTo(() => _jsonFactoryMock.CreateJsonTextReader(A<string>.Ignored)).Returns(jsonTextReaderMock);

            return new JsonPathPatternExtractor(_jsonFactoryMock);
        }
    }
}
