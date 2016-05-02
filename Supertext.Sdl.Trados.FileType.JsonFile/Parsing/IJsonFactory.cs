namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public interface IJsonFactory
    {
        IJsonTextReader CreateJsonTextReader(string file);
    }
}
