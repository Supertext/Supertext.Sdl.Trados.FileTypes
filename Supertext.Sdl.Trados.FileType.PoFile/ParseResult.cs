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

        public override bool Equals(object obj)
        {
            var other = obj as ParseResult;
            return other != null && other.LineType == LineType && string.Equals(LineContent, other.LineContent);
        }

        protected bool Equals(ParseResult other)
        {
            return LineType == other.LineType && string.Equals(LineContent, other.LineContent);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) LineType*397) ^ (LineContent != null ? LineContent.GetHashCode() : 0);
            }
        }
    }
}