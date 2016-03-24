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
                { @"%\d+", InlineType.Placeholder }
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
                var textBefore = value.Substring(lastIndex, match.Index - lastIndex);
                var inlineContent = value.Substring(match.Index, match.Length);

                var type = InlineType.Text;
                for (var i = 1; i < match.Groups.Count; ++i)
                {
                    if (match.Groups[i].Value == match.Value)
                    {
                        type = _types[i - 1];
                    }
                }

                fragments.Add(new Fragment(InlineType.Text, textBefore));
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

    public enum InlineType
    {
        Placeholder,
        StartTag,
        EndTag,
        Text
    }
}
