using System.IO;
using System.Linq;

namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public class FileHelper : IFileHelper
    {
        public IStreamReader GetStreamReader(string path)
        {
            return new StreamReaderWrapper(path);
        }

        public IExtendedStreamReader GetExtendedStreamReader(string path)
        {
            return new ExtendedStreamReader(GetStreamReader(path));
        }

        public IStreamWriter GetStreamWriter(string path)
        {
            return new StreamWriterWrapper(path);
        }

        public int GetNumberOfLines(string path)
        {
            return File.ReadLines(path).Count();
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}