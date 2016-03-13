using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ExtendedFileReaderTest
    {
        private const string TestFilePath = "sample_file_ok";

        [Test]
        public void ReadLinesWithEofLine_ReturnsAllLinesOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.ReadLinesWithEofLine(TestFilePath).ToList();

            // Assert
            result.FirstOrDefault(line => line == "line 1").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 2").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 3").Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ReadLinesWithEofLine_WhenGettingLastLine_ReturnsEndOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.ReadLinesWithEofLine(TestFilePath);

            // Assert
            result.Last().Should().Be(MarkerLines.EndOfFile);
        }

        private ExtendedFileReader CreateTestee(string testString)
        {
            var fileHelper = A.Fake<IFileHelper>();
            var lines = testString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            A.CallTo(() => fileHelper.GetTotalNumberOfLines(TestFilePath)).Returns(lines.Length);
            A.CallTo(() => fileHelper.ReadLines(TestFilePath)).Returns(lines);

            return new ExtendedFileReader(fileHelper);
        }
    }
}
