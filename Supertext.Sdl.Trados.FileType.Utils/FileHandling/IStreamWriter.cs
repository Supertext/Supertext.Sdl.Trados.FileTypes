namespace Supertext.Sdl.Trados.FileType.Utils.FileHandling
{
    public interface IStreamWriter
    {
        void Write(string value);

        void WriteLine();

        void WriteLine(string value);

        void Close();

        void Dispose();
    }
}