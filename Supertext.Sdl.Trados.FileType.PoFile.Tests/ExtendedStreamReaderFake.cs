using System;
using FakeItEasy;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    internal class ExtendedStreamReaderFake
    {
        public static IExtendedStreamReader Create(string testString)
        {
            var lines = (testString + Environment.NewLine + MarkerLines.EndOfFile).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var counter = 0;
            var extendedStreamReaderMock = A.Fake<IExtendedStreamReader>();
            A.CallTo(() => extendedStreamReaderMock.ReadLineWithEofLine()).ReturnsLazily(() => counter < lines.Length ? lines[counter++] : null);
            A.CallTo(() => extendedStreamReaderMock.GetLinesWithEofLine()).ReturnsLazily(() =>
            {
                counter = lines.Length;
                return lines;
            });
            A.CallTo(() => extendedStreamReaderMock.CurrentLineNumber).ReturnsLazily(() => counter);
            return extendedStreamReaderMock;
        }
    }
}