using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ITextProcessor
    {
        IList<Fragment> Process(string value);
    }

    public class TextProcessor : ITextProcessor
    {
        private readonly Dictionary<string, InlineType> _embeddedContentPatterns;
        private readonly List<InlineType> _types;

        public static Dictionary<string, InlineType> DefaultEmbeddedContentRegexs = new Dictionary<string, InlineType>
            {
                { @"%\d+", InlineType.Placeholder },
                { @"<[a-z][a-z0-9]*[^<>]*>", InlineType.StartTag },
                { @"</[a-z][a-z0-9]*[^<>]*>", InlineType.EndTag }
            };

        public TextProcessor(Dictionary<string, InlineType> embeddedContentPatterns)
        {
            _embeddedContentPatterns = embeddedContentPatterns;
            _types = new List<InlineType>(_embeddedContentPatterns.Values);
        }

        public IList<Fragment> Process(string value)
        {
            var patterns = "(" + string.Join(")|(", _embeddedContentPatterns.Keys) + ")";
            var regex = new Regex(patterns);

            var matches = regex.Matches(value);

            var fragments = new List<Fragment>();
            var lastIndex = 0;

            foreach (Match match in matches)
            {
                var type = InlineType.Text;
                for (var i = 1; i < match.Groups.Count; ++i)
                {
                    if (match.Groups[i].Value == match.Value)
                    {
                        type = _types[i - 1];
                    }
                }

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
    }
}
