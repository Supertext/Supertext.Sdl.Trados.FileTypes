using System;

namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IReader : IDisposable
    {
        string ReadLine();

        void Close();
    }
}