namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IDotNetFactory
    {
        IReader CreateStreamReader(string filePath);
    }
}