namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IFileHelper
    {
        int GetTotalNumberOfLines(string path);

        IStreamReader GetStreamReader(string path);

        IExtendedStreamReader GetExtendedStreamReader(string path);

        IStreamWriter GetStreamWriter(string path);
    }
}