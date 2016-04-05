namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public interface ILineParsingSession
    {
        IParseResult Parse(string line);
    }
}