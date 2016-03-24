using System;
using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IExtendedStreamReader : IDisposable
    {
        int CurrentLineNumber { get; }

        string ReadLineWithEofLine();

        IEnumerable<Line> GetLinesWithEofLine();

        void Close();
    }
}