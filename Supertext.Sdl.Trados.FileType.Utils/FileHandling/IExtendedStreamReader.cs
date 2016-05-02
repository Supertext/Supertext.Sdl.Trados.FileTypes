using System;
using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public interface IExtendedStreamReader : IDisposable
    {
        int CurrentLineNumber { get; }

        string ReadLineWithEofLine();

        IEnumerable<string> GetLinesWithEofLine();

        void Close();
    }
}