namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public interface IParseResult
    {
        LineType LineType { get; }

        string LineContent { get; }
    }
}