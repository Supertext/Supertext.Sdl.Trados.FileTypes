namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IParseResult
    {
        LineType LineType { get; }

        string LineContent { get; }
    }
}