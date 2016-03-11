using System.IO;
using System.Linq;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class FileHelper : IFileHelper
    {
        public IReader CreateStreamReader(string path)
        {
            return new StreamReaderWrapper(path);
        }

        public int GetTotalNumberOfLines(string path)
        {
            return File.ReadLines(path).Count();
        }
    }
}