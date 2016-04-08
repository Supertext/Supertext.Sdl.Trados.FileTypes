using System.Collections.Generic;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    //TODO small but complicated, needs to be refactored to easy understandable code
    public class TextProcessor : ITextProcessor
    {
        private List<InlineType> _types;
        private List<MatchRule> _rules;
        private Regex _regex;

        public TextProcessor()
        {
            _types = new List<InlineType> {InlineType.Text};
            _regex = new Regex(".*");
        }

        public void InitializeWith(IEnumerable<MatchRule> matchRules)
        {
            _types = new List<InlineType>();
            _rules = new List<MatchRule>();

            var fullPattnern = "";

            foreach (var matchRule in matchRules)
            {
                if (matchRule.TagType == MatchRule.TagTypeOption.TagPair)
                {
                    fullPattnern += "(" + matchRule.StartTagRegexValue + ")|";
                    _types.Add(InlineType.StartTag);
                    _rules.Add(matchRule);
                    fullPattnern += "(" + matchRule.EndTagRegexValue + ")|";
                    _types.Add(InlineType.EndTag);
                    _rules.Add(matchRule);
                }
                else if (matchRule.TagType == MatchRule.TagTypeOption.Placeholder)
                {
                    fullPattnern += "(" + matchRule.StartTagRegexValue + ")|";
                    _types.Add(InlineType.Placeholder);
                    _rules.Add(matchRule);
                }
            }

            fullPattnern = fullPattnern.Substring(0, fullPattnern.Length - 1);

            _regex = new Regex(fullPattnern);
        }

        //Todo: check performance, maybe slow with a lot of patterns
        public IList<Fragment> Process(string value)
        {
            var matches = _regex.Matches(value);

            var fragments = new List<Fragment>();
            var lastIndex = 0;

            foreach (Match match in matches)
            {
                var type = GetInlineTypeFor(match.Value, match.Groups);
                var matchRule = GetMatchRuleFor(match.Value, match.Groups);

                var textBeforeLength = match.Index - lastIndex;

                if (textBeforeLength > 0)
                {
                    var textBefore = value.Substring(lastIndex, textBeforeLength);
                    fragments.Add(new Fragment(InlineType.Text, textBefore));
                }
 
                var inlineContent = value.Substring(match.Index, match.Length);
                fragments.Add(new Fragment(type, inlineContent, matchRule));

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
            for (var i = 1; i < groups.Count; ++i)
            {
                if (groups[i].Value == matchValue)
                {
                   return _types[i - 1];
                }
            }
           
            return InlineType.Text;
        }

        private MatchRule GetMatchRuleFor(string matchValue, GroupCollection groups)
        {
            for (var i = 1; i < groups.Count; ++i)
            {
                if (groups[i].Value == matchValue)
                {
                    return _rules[i - 1];
                }
            }

            return null;
        }
    }
}
