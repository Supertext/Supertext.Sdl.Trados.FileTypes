using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public class Fragment
    {
        public Fragment(InlineType inlineType, string content, MatchRule matchRule = null)
        {
            InlineType = inlineType;
            Content = content;
            MatchRule = matchRule;
        }

        public InlineType InlineType { get; }

        public string Content { get; }

        public MatchRule MatchRule { get; }
    }
}