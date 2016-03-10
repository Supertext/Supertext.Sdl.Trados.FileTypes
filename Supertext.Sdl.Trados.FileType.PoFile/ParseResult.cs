namespace Supertext.Sdl.Trados.FileType.PoFile
{
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