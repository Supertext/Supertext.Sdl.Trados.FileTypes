namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILineParsingSession
    {
        IParseResult Parse(string line);
    }
}