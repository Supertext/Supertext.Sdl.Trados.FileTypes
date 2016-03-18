using System;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IStreamReader : IDisposable
    {
        string ReadLine();

        void Close();
    }
}