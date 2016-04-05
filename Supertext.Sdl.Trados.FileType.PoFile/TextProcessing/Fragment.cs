namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public struct Fragment
    {
        public InlineType InlineType;
        public string Content;

        public Fragment(InlineType inlineType, string content)
        {
            InlineType = inlineType;
            Content = content;
        }
    }
}