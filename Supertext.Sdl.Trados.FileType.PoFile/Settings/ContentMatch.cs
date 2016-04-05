using System;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public class ContentMatch
    {
        public MatchRule MatchRule { get; set; }

        public Int32 Index { get; set; }

        public string Value { get; set; }

        public TagType Type { get; set; }

        public enum TagType
        {
            Placeholder,
            TagPairOpening,
            TagPairClosing
        }
    }
}
