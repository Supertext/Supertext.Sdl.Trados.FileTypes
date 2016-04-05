using System.Windows.Forms;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    public class RegexRuleListItem : ListViewItem
    {
        private MatchRule _matchRule;

        public RegexRuleListItem(MatchRule rule)
        {
            Init(rule);
        }

        private void Init(MatchRule rule)
        {
            _matchRule = rule;

            SubItems.Clear();

            //first row is actual item, all others are "sub-items"
            Text = rule.StartTagRegexValue;
            SubItems.Add(rule.EndTagRegexValue);

            string tagType = rule.TagType == MatchRule.TagTypeOption.TagPair
                ? "Tag Pair"
                : "Placeholder";
            SubItems.Add(tagType);

            string isTranslatable = rule.IsContentTranslatable
                ? "Translatable"
                : "Not translatable";
            SubItems.Add(isTranslatable);
        }

        public void SetRule(MatchRule rule)
        {
            Init(rule);
        }

        public MatchRule GetRule()
        {
            return _matchRule;
        }
    }
}
