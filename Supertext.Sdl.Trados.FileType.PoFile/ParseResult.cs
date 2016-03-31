namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IParseResult
    {
        LineType LineType { get; }

        string LineContent { get; }

        string RawContent { get;}
    }

    public class ParseResult : IParseResult
    {
        public ParseResult(LineType lineType, string lineContent, string rawContent)
        {
            LineType = lineType;
            LineContent = lineContent;
            RawContent = rawContent;
        }

        public LineType LineType { get; }

        public string LineContent { get; }

        public string RawContent { get; }
    }
}