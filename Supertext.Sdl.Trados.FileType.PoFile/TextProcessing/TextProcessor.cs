using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    //TODO small but complicated, needs to be refactored to easy understandable code
    public class TextProcessor : ITextProcessor
    {
        private List<InlineType> _types;
        private Regex _regex;

        public static Dictionary<string, InlineType> DefaultEmbeddedContentPatterns = new Dictionary<string, InlineType>
            {
                { @"%\d+", InlineType.Placeholder },
                { @"%\w+", InlineType.Placeholder },
                { @"\$\w+", InlineType.Placeholder },
                { @"<[a-z][a-z0-9]*[^<>]*>", InlineType.StartTag },
                { @"</[a-z][a-z0-9]*[^<>]*>", InlineType.EndTag }
            };

        //Todo: check performance, maybe slow with a lot of patterns
        public IList<Fragment> Process(string value, Dictionary<string, InlineType> embeddedContentPatterns)
        {
            if (_regex == null)
            {
                _regex = new Regex("(" + string.Join(")|(", embeddedContentPatterns.Keys) + ")");
                _types = new List<InlineType>(embeddedContentPatterns.Values);
            }
            
            var matches = _regex.Matches(value);

            var fragments = new List<Fragment>();
            var lastIndex = 0;

            foreach (Match match in matches)
            {
                var type = GetInlineTypeFor(match.Value, match.Groups);

                var textBeforeLength = match.Index - lastIndex;

                if (textBeforeLength > 0)
                {
                    var textBefore = value.Substring(lastIndex, textBeforeLength);
                    fragments.Add(new Fragment(InlineType.Text, textBefore));
                }
 
                var inlineContent = value.Substring(match.Index, match.Length);
                fragments.Add(new Fragment(type, inlineContent));

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex != value.Length)
            {
                fragments.Add(new Fragment(InlineType.Text, value.Substring(lastIndex)));
            }

            return fragments;
        }

        private InlineType GetInlineTypeFor(string matchValue, GroupCollection groups)
        {
            var type = InlineType.Text;
            for (var i = 1; i < groups.Count; ++i)
            {
                if (groups[i].Value == matchValue)
                {
                    type = _types[i - 1];
                }
            }
            return type;
        }
    }
}
