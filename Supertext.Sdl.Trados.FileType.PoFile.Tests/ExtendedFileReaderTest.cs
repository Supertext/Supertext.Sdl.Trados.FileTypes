using System;
using System.IO;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ExtendedFileReaderTest
    {
        private const string TestFilePath = "sample_file_ok";

        [Test]
        public void GetLinesWithEofLine_ReturnsAllLinesOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.GetLinesWithEofLine(TestFilePath).ToList();

            // Assert
            result.FirstOrDefault(line => line == "line 1").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 2").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 3").Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GetLinesWithEofLine_WhenGettingLastLine_ReturnsEndOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.GetLinesWithEofLine(TestFilePath);

            // Assert
            result.Last().Should().Be(MarkerLines.EndOfFile);
        }

        private ExtendedFileReader CreateTestee(string testString)
        {
            var fileHelper = A.Fake<IFileHelper>();
            var lines = testString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            A.CallTo(() => fileHelper.GetTotalNumberOfLines(TestFilePath)).Returns(lines.Length);

            var streamReaderMock = A.Fake<IStreamReader>();
            A.CallTo(() => streamReaderMock.ReadLine()).Returns(null);
            A.CallTo(() => streamReaderMock.ReadLine()).ReturnsNextFromSequence(lines);
            A.CallTo(() => fileHelper.GetStreamReader(TestFilePath)).Returns(streamReaderMock);

            return new ExtendedFileReader(fileHelper);
        }
    }
}
