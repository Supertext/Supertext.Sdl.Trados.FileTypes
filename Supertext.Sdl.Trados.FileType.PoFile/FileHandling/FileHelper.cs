using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public class FileHelper : IFileHelper
    {
        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }

        public int GetTotalNumberOfLines(string path)
        {
            return File.ReadLines(path).Count();
        }

        public IStreamReader GetStreamReader(string path)
        {
            return new StreamReaderWrapper(path);
        }

        public IStreamWriter GetStreamWriter(string path)
        {
            return new StreamWriterWrapper(path);
        }
    }
}