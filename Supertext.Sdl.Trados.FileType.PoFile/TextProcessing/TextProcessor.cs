using System.Collections.Generic;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    //TODO small but complicated, needs to be refactored to easy understandable code
    public class TextProcessor : ITextProcessor
    {
        private List<InlineType> _types;
        private Regex _regex;

        public TextProcessor()
        {
            _types = new List<InlineType> {InlineType.Text};
            _regex = new Regex(".*");
        }

        //Todo: check performance, maybe slow with a lot of patterns
        public void InitializeWith(IEnumerable<MatchRule> matchRules)
        {
            _types = new List<InlineType>();

            var fullPattnern = "";

            foreach (var matchRule in matchRules)
            {
                if (matchRule.TagType == MatchRule.TagTypeOption.TagPair)
                {
                    fullPattnern += "(" + matchRule.StartTagRegexValue + ")|";
                    _types.Add(InlineType.StartTag);
                    fullPattnern += "(" + matchRule.EndTagRegexValue + ")|";
                    _types.Add(InlineType.EndTag);
                }
                else if (matchRule.TagType == MatchRule.TagTypeOption.Placeholder)
                {
                    fullPattnern += "(" + matchRule.StartTagRegexValue + ")|";
                    _types.Add(InlineType.Placeholder);
                }
            }

            fullPattnern = fullPattnern.Substring(0, fullPattnern.Length - 1);

            _regex = new Regex(fullPattnern);
        }   

        public IList<Fragment> Process(string value)
        {
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
