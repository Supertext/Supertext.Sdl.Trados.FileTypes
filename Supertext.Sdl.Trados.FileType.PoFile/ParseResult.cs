namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IParseResult
    {
        LineType LineType { get; }

        string LineContent { get; }

        string LineKeyword { get;}
    }

    public class ParseResult : IParseResult
    {
        public ParseResult(LineType lineType, string lineContent, string lineKeyword)
        {
            LineType = lineType;
            LineContent = lineContent;
            LineKeyword = lineKeyword;
        }

        public LineType LineType { get; }

        public string LineContent { get; }

        public string LineKeyword { get; }
    }
}