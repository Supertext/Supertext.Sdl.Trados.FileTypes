using System.Collections.Generic;
using System.IO;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IFileHelper
    {
        int GetTotalNumberOfLines(string path);

        IEnumerable<string> ReadLines(string path);

        IStreamWriter GetStreamWriter(string path);
    }
}