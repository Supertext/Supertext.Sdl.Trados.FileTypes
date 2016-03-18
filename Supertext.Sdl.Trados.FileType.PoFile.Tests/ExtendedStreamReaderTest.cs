﻿using System;
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
        public void ReadLine_ReturnsAllLinesOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);

            // Act
            var result1 = testee.ReadLine();
            var result2 = testee.ReadLine();
            var result3 = testee.ReadLine();

            // Assert
            result1.Should().Be("line 1");
            result2.Should().Be("line 2");
            result3.Should().Be("line 3");
        }

        [Test]
        public void ReadLine_WhenGettingLastLine_ReturnsEndOfFile()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
             testee.ReadLine();
             testee.ReadLine();
             testee.ReadLine();

            // Act

            var result = testee.ReadLine();

            // Assert
            result.Should().Be(MarkerLines.EndOfFile);
        }

        [Test]
        public void ReadLine_WhenGettingAfterEndOfFileLine_ReturnsNull()
        {
            // Arrange
            var testString = @"line 1
line 2
line 3";
            var testee = CreateTestee(testString);
            testee.ReadLine();
            testee.ReadLine();
            testee.ReadLine();
            testee.ReadLine();

            // Act

            var result = testee.ReadLine();

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
