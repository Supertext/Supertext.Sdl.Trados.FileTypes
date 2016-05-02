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
            var tagMatches = GetTagMatches(value);

            return CreateFragments(value, tagMatches);
        }

        private TagMatch[] GetTagMatches(string value)
        {
            var tagMatches = new TagMatch[value.Length];

            foreach (var matchRule in _matchRules)
            {
                var startTagRegexMatches = new Regex(matchRule.StartTagRegexValue).Matches(value);

                if (matchRule.TagType == MatchRule.TagTypeOption.Placeholder)
                {
                    AddTagMatches(tagMatches, startTagRegexMatches, InlineType.Placeholder, matchRule);
                    continue;
                }

                var endTagRegexMatches = new Regex(matchRule.EndTagRegexValue).Matches(value);
                var startTagInlineType = InlineType.StartTag;
                var endTagInlineType = InlineType.EndTag;

                if (startTagRegexMatches.Count != endTagRegexMatches.Count)
                {
                    startTagInlineType = InlineType.Placeholder;
                    endTagInlineType = InlineType.Placeholder;
                }

                AddTagMatches(tagMatches, startTagRegexMatches, startTagInlineType, matchRule);
                AddTagMatches(tagMatches, endTagRegexMatches, endTagInlineType, matchRule);
            }
            return tagMatches;
        }

        private static IList<IFragment> CreateFragments(string value, IReadOnlyList<TagMatch> tagMatches)
        {
            var fragments = new List<IFragment>();

            var stringFragmentBuilder = new StringBuilder();

            for (var i = 0; i < tagMatches.Count; ++i)
            {
                var tagMatch = tagMatches[i];

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

        private static void AddTagMatches(IList<TagMatch> tagMatches, IEnumerable regexMatches, InlineType inlineType,
            MatchRule matchRule)
        {
            foreach (Match regexMatch in regexMatches)
            {
                var currentTagMatch = new TagMatch(inlineType, regexMatch, matchRule);
                var lastTagMatch = currentTagMatch.Start == 0 ? null : tagMatches[currentTagMatch.Start - 1];

                if (lastTagMatch != null && lastTagMatch.End >= currentTagMatch.Start &&
                    lastTagMatch.Length > currentTagMatch.Length)
                {
                    continue;
                }

                var indexAfterCurrentTagMatch = currentTagMatch.Start + currentTagMatch.Length;
                var nextTagMatch = indexAfterCurrentTagMatch == tagMatches.Count
                    ? null
                    : tagMatches[indexAfterCurrentTagMatch];

                if (nextTagMatch != null && nextTagMatch.Start <= currentTagMatch.End &&
                    nextTagMatch.Length > currentTagMatch.Length)
                {
                    continue;
                }

                for (var i = currentTagMatch.Start; i <= currentTagMatch.End; ++i)
                {
                    tagMatches[i] = currentTagMatch;
                }
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