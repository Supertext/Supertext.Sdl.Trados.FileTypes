using System.IO;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    internal class StringReaderWrapper : IReader
    {
        private readonly StringReader _stringReader;

        public StringReaderWrapper(string text)
        {
            _stringReader = new StringReader(text);
        }

        public void Dispose()
        {
            _stringReader.Dispose();
        }

        public string ReadLine()
        {
            return _stringReader.ReadLine();
        }

        public void Close()
        {
            _stringReader.Close();
        }
    }
}