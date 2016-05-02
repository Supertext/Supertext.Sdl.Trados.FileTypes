using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

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
                ? PluginResources.Tag_Pair
                : PluginResources.Placeholder;
            SubItems.Add(tagType);

            string isTranslatable = rule.IsContentTranslatable
                ? PluginResources.Translatable
                : PluginResources.Not_Translatable;
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
