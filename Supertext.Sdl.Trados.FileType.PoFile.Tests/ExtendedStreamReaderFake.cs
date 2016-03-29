using System;
using System.Collections.Generic;
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

        public IEnumerable<string> GetLinesWithEofLine()
        {
            CurrentLineNumber = _lines.Length;
            return _lines;
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}