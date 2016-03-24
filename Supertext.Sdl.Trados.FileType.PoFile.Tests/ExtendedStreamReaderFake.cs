using System;
using System.Collections.Generic;
using System.Linq;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    internal class ExtendedStreamReaderFake : IExtendedStreamReader
    {
        private readonly string[] _lines;

        public ExtendedStreamReaderFake(string testString)
        {
            _lines = (testString + Environment.NewLine + MarkerLines.EndOfFile).Split(new[] {Environment.NewLine},
                StringSplitOptions.None);
        }

        public int CurrentLineNumber { get; private set; }

        public string ReadLineWithEofLine()
        {
            if (CurrentLineNumber == _lines.Length)
            {
                return null;
            }

            return _lines[CurrentLineNumber++];
        }

        public IEnumerable<Line> GetLinesWithEofLine()
        {
            return _lines.Select(line => new Line(++CurrentLineNumber, line));
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}