using System;
using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IExtendedStreamReader : IDisposable
    {
        string ReadLineWithEofLine();

        IEnumerable<string> GetLinesWithEofLine();

        void Close();
    }
}