using System.IO;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class StreamReaderWrapper : IStreamReader
    {
        private readonly StreamReader _streamReader;

        public StreamReaderWrapper(string filePath)
        {
            _streamReader = new StreamReader(filePath);
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }
    }
}