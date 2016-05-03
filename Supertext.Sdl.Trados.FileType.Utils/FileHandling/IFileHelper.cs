namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public interface IFileHelper
    {
        IStreamReader GetStreamReader(string path);

        IExtendedStreamReader GetExtendedStreamReader(string path);

        IStreamWriter GetStreamWriter(string path);

        int GetNumberOfLines(string originalFilePath);

        void WriteAllText(string path, string content);
    }
}