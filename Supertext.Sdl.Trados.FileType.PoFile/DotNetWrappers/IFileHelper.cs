using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IFileHelper
    {
        int GetTotalNumberOfLines(string path);

        IEnumerable<string> ReadLines(string path);
    }
}