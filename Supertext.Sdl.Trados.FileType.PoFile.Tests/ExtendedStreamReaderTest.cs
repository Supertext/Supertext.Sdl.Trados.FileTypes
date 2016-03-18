using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ExtendedStreamReaderTest
    {
        [Test]
        public void ReadLineWithEofLine_ReturnsAllLinesOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);

            // Act
            var result1 = testee.ReadLineWithEofLine();
            var result2 = testee.ReadLineWithEofLine();
            var result3 = testee.ReadLineWithEofLine();

            // Assert
            result1.Should().Be("line 1");
            result2.Should().Be("line 2");
            result3.Should().Be("line 3");
        }

        [Test]
        public void ReadLineWithEofLine_WhenGettingLastLine_ReturnsEndOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
             testee.ReadLineWithEofLine();
             testee.ReadLineWithEofLine();
             testee.ReadLineWithEofLine();

            // Act

            var result = testee.ReadLineWithEofLine();

            // Assert
            result.Should().Be(MarkerLines.EndOfFile);
        }

        [Test]
        public void ReadLineWithEofLine_WhenAfterEndOfFileLine_ReturnsNull()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
            testee.ReadLineWithEofLine();
            testee.ReadLineWithEofLine();
            testee.ReadLineWithEofLine();
            testee.ReadLineWithEofLine();

            // Act

            var result = testee.ReadLineWithEofLine();

            // Assert
            result.Should().BeNull();
        }

        private ExtendedStreamReader CreateTestee(string testString)
        {
            var lines = testString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var streamReaderMock = A.Fake<IStreamReader>();
            A.CallTo(() => streamReaderMock.ReadLine()).Returns(null);
            A.CallTo(() => streamReaderMock.ReadLine()).ReturnsNextFromSequence(lines);

            return new ExtendedStreamReader(streamReaderMock);
        }
    }
}
