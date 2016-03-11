namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public interface IFileHelper
    {
        IReader CreateStreamReader(string path);

        int GetTotalNumberOfLines(string path);
    }
}