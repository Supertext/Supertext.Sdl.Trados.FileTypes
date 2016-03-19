namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public interface IFileHelper
    {
        IStreamReader GetStreamReader(string path);

        IExtendedStreamReader GetExtendedStreamReader(string path);

        IStreamWriter GetStreamWriter(string path);
    }
}