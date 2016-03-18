using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IExtendedStreamReader
    {
        string ReadLineWithEofLine();

        IEnumerable<string> GetLinesWithEofLine();
    }
}