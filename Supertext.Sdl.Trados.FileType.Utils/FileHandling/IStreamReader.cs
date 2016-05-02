using System;

namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public interface IStreamReader : IDisposable
    {
        string ReadLine();

        void Close();
    }
}