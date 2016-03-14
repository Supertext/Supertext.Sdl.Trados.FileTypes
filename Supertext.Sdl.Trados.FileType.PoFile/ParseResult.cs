namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IParseResult
    {
        LineType LineType { get; }

        string LineContent { get; }
    }

    public class ParseResult : IParseResult
    {
        public ParseResult(LineType lineType, string lineContent)
        {
            LineType = lineType;
            LineContent = lineContent;
        }

        public LineType LineType { get; }

        public string LineContent { get; }
    }
}