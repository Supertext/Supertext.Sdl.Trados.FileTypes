﻿using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ExtendedStreamReaderTest
    {
        [Test]
        public void ReadLineWithEofLine_ShouldReturnAllLinesOfFile()
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
        public void ReadLineWithEofLine_WhenGettingLastLine_ShouldReturnEndOfFile()
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
        public void ReadLineWithEofLine_WhenAfterEndOfFileLine_ShouldReturnNull()
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

        [Test]
        public void GetLinesWithEofLine_ShouldReturnAllLinesOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.GetLinesWithEofLine().ToList();

            // Assert
            result.FirstOrDefault(line => line == "line 1").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 2").Should().NotBeNullOrEmpty();
            result.FirstOrDefault(line => line == "line 3").Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GetLinesWithEofLine_WhenGettingLastLine_ShouldReturnEndOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.GetLinesWithEofLine().ToList();

            // Assert
            result.Last().Should().Be(MarkerLines.EndOfFile);
        }

        [Test]
        public void GetCurrentLineNumber_WhenNothingRead_ShouldReturnZero()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);

            // Act
            var result = testee.CurrentLineNumber;

            // Assert
            result.Should().Be(0);
        }

        [Test]
        public void GetCurrentLineNumber_WhenReadALineWithReadLine_ShouldReturnOne()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
            testee.ReadLineWithEofLine();

            // Act
            var result = testee.CurrentLineNumber;

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void GetCurrentLineNumber_WhenReadALineWithGetLine_ShouldReturnOne()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
            testee.GetLinesWithEofLine().First();

            // Act
            var result = testee.CurrentLineNumber;

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void GetCurrentLineNumber_WhenReadAllLinesWithReadLine_ShouldReturnLastLineNumber()
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
            testee.ReadLineWithEofLine();
            testee.ReadLineWithEofLine();

            // Act
            var result = testee.CurrentLineNumber;

            // Assert
            result.Should().Be(4);
        }

        [Test]
        public void GetCurrentLineNumber_WhenReadAllLinesWithGetLines_ShouldReturnLastLineNumber()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
            testee.GetLinesWithEofLine().ToList();

            // Act
            var result = testee.CurrentLineNumber;

            // Assert
            result.Should().Be(4);
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
