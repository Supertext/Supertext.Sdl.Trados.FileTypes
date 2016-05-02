using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    //Todo refactor
    public class TextProcessor : ITextProcessor
    {
        private List<MatchRule> _matchRules = new List<MatchRule>();

        public void InitializeWith(IEnumerable<MatchRule> matchRules)
        {
            _matchRules = matchRules.ToList();
        }

        //Todo: check performance, maybe slow with a lot of patterns
        public IList<IFragment> Process(string value)
        {
            var tagMatchesPerChar = GetTagMatchesPerChar(value);

            return CreateFragments(value, tagMatchesPerChar);
        }

        private TagMatch[] GetTagMatchesPerChar(string value)
        {
            var tagMatchesPerChar = new TagMatch[value.Length];

            foreach (var matchRule in _matchRules)
            {
                var startTagRegexMatches = new Regex(matchRule.StartTagRegexValue).Matches(value);

                if (matchRule.TagType == MatchRule.TagTypeOption.Placeholder)
                {
                    AddTagMatches(tagMatchesPerChar, startTagRegexMatches, InlineType.Placeholder, matchRule);
                    continue;
                }

                var endTagRegexMatches = new Regex(matchRule.EndTagRegexValue).Matches(value);
                var startTagInlineType = InlineType.StartTag;
                var endTagInlineType = InlineType.EndTag;

                var startTagMatches = GetTagMatches(tagMatchesPerChar, startTagRegexMatches, startTagInlineType, matchRule).Count();
                var endTagMatches = GetTagMatches(tagMatchesPerChar, endTagRegexMatches, endTagInlineType, matchRule).Count();

                if (startTagMatches != endTagMatches)
                {
                    startTagInlineType = InlineType.Placeholder;
                    endTagInlineType = InlineType.Placeholder;
                }

                AddTagMatches(tagMatchesPerChar, startTagRegexMatches, startTagInlineType, matchRule);
                AddTagMatches(tagMatchesPerChar, endTagRegexMatches, endTagInlineType, matchRule);
            }
            return tagMatchesPerChar;
        }

        private static IList<IFragment> CreateFragments(string value, IReadOnlyList<TagMatch> tagMatchesPerChar)
        {
            var fragments = new List<IFragment>();

            var stringFragmentBuilder = new StringBuilder();

            for (var i = 0; i < tagMatchesPerChar.Count; ++i)
            {
                var tagMatch = tagMatchesPerChar[i];

                if (tagMatch == null)
                {
                    stringFragmentBuilder.Append(value[i]);
                    continue;
                }

                if (i == tagMatch.Start && stringFragmentBuilder.Length > 0)
                {
                    fragments.Add(new Fragment(InlineType.Text, stringFragmentBuilder.ToString(), SegmentationHint.Include, true));
                    stringFragmentBuilder.Clear();
                }

                stringFragmentBuilder.Append(value[i]);

                if (i != tagMatch.End)
                {
                    continue;
                }

                fragments.Add(new Fragment(tagMatch.InlineType, stringFragmentBuilder.ToString(), tagMatch.MatchRule.SegmentationHint, tagMatch.MatchRule.IsContentTranslatable));
                stringFragmentBuilder.Clear();
            }

            if (stringFragmentBuilder.Length > 0)
            {
                fragments.Add(new Fragment(InlineType.Text, stringFragmentBuilder.ToString(), SegmentationHint.Include, true));
            }

            return fragments;
        }

        private static void AddTagMatches(IList<TagMatch> tagMatchesPerChar, IEnumerable regexMatches, InlineType inlineType,
            MatchRule matchRule)
        {
            var newTagMatches = GetTagMatches(tagMatchesPerChar, regexMatches, inlineType, matchRule);

            foreach (var tagMatch in newTagMatches)
            {
                for (var i = tagMatch.Start; i <= tagMatch.End; ++i)
                {
                    tagMatchesPerChar[i] = tagMatch;
                }
            }
        }

        private static IEnumerable<TagMatch> GetTagMatches(IList<TagMatch> tagMatchesPerChar, IEnumerable regexMatches, InlineType inlineType,
            MatchRule matchRule)
        {
            foreach (Match regexMatch in regexMatches)
            {
                var newTagMatch = new TagMatch(inlineType, regexMatch, matchRule);

                var currentTagMatch = tagMatchesPerChar[newTagMatch.Start];
                if (currentTagMatch != null && currentTagMatch.End == newTagMatch.End)
                {
                    continue;
                }

                var lastTagMatch = newTagMatch.Start == 0 ? null : tagMatchesPerChar[newTagMatch.Start - 1];
                if (lastTagMatch != null && lastTagMatch.End >= newTagMatch.Start)
                {
                    continue;
                }

                var indexAfterCurrentTagMatch = newTagMatch.Start + newTagMatch.Length;
                var nextTagMatch = indexAfterCurrentTagMatch == tagMatchesPerChar.Count
                    ? null
                    : tagMatchesPerChar[indexAfterCurrentTagMatch];

                if (nextTagMatch != null && nextTagMatch.Start <= newTagMatch.End)
                {
                    continue;
                }

                yield return newTagMatch;
            }
        }

        private class TagMatch
        {
            private readonly Match _match;

            public TagMatch(InlineType inlineType, Match match, MatchRule matchRule)
            {
                _match = match;
                InlineType = inlineType;
                MatchRule = matchRule;
            }

            public InlineType InlineType { get; }

            public MatchRule MatchRule { get; }

            public int Start
            {
                get { return _match.Index; }
            }

            public int Length
            {
                get { return _match.Length; }
            }

            public int End
            {
                get { return Start + Length - 1; }
            }
        }
    }
}