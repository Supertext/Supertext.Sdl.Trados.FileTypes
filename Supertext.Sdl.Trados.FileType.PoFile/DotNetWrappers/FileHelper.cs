using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class FileHelper : IFileHelper
    {
        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }

        public IStreamWriter GetStreamWriter(string path)
        {
            return new StreamWriterWrapper(path);
        }

        public int GetTotalNumberOfLines(string path)
        {
            return File.ReadLines(path).Count();
        }
    }
}