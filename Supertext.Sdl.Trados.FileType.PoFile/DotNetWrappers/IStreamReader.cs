using System;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IStreamReader : IDisposable
    {
        string ReadLine();

        void Close();
    }
}