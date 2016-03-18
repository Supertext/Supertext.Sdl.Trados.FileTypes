using System.Collections.Generic;
using System.IO;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IFileHelper
    {
        int GetTotalNumberOfLines(string path);

        IStreamReader GetStreamReader(string path);

        IStreamWriter GetStreamWriter(string path);
    }
}