namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public struct Fragment
    {
        public InlineType InlineType;
        public string Content;
        public bool IsContentTranslatable;

        public Fragment(InlineType inlineType, string content, bool isContentTranslatable = true)
        {
            InlineType = inlineType;
            Content = content;
            IsContentTranslatable = isContentTranslatable;
        }
    }
}