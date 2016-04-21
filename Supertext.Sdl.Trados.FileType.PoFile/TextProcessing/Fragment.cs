using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public struct Fragment
    {
        public InlineType InlineType;
        public string Content;
        public MatchRule MatchRule;

        public Fragment(InlineType inlineType, string content, MatchRule matchRule = null)
        {
            InlineType = inlineType;
            Content = content;
            MatchRule = matchRule;
        }

        public override string ToString()
        {
            return InlineType + ": " + Content;
        }
    }
}