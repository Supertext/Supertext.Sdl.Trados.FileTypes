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
        public void ReadLines_WhenGettingFirstLine_ReturnsBeginOfFile()
        {
            // Arrange
            var testee = CreateTestee(string.Empty);

            // Act
            var result = testee.ReadLines(TestFilePath);

            // Assert
            result.First().Should().Be(LineType.BeginOfFile.ToString());
        }

        [Test]
        public void ReadLines_ReturnsAllLinesOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.ReadLines(TestFilePath).ToList();

            // Assert
            result.FirstOrDefault(line => line == "line 1").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 2").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 3").Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ReadLines_WhenGettingLastLine_ReturnsEndOfFile()
        {
            // Arrange
            var testString = @"
line 1
line 2
line 3
";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.ReadLines(TestFilePath);

            // Assert
            result.Last().Should().Be(LineType.EndOfFile.ToString());
        }

        private ExtendedFileReader CreateTestee(string testString)
        {
            var fileHelper = A.Fake<IFileHelper>();

            var streamReaderFake = new StringReaderWrapper(testString);
            A.CallTo(() => fileHelper.CreateStreamReader(TestFilePath)).Returns(streamReaderFake);

            return new ExtendedFileReader(fileHelper);
        }
    }
}
