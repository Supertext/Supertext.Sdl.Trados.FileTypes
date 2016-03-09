using System.IO;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class StreamReaderWrapper : IReader
    {
        private readonly StreamReader _streamReader;

        public StreamReaderWrapper(string filePath)
        {
            _streamReader = new StreamReader(filePath);
        }

        public bool EndOfStream => _streamReader.EndOfStream;

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }

        public void Close()
        {
            _streamReader.Close();
        }
    }
}