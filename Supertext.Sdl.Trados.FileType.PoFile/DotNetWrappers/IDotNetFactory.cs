namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IDotNetFactory
    {
        IStreamReader CreateStreamReader(string filePath);
    }
}