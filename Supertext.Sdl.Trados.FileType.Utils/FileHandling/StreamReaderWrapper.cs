using System.IO;

namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public class StreamReaderWrapper : IStreamReader
    {
        private readonly StreamReader _streamReader;

        public StreamReaderWrapper(string path)
        {
            _streamReader = new StreamReader(path);
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }

        public void Close()
        {
            _streamReader.Close();
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}