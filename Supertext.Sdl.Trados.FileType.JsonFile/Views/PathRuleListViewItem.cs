﻿using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Views
{
    internal class PathRuleListViewItem : ListViewItem
    {
        private PathRule _pathRule;

        public PathRuleListViewItem(PathRule pathRule)
        {
            PathRule = pathRule;
        }

        public PathRule PathRule
        {
            get { return _pathRule; }
            set
            {
                _pathRule = value;
                OnPathRuleChanged();
            }
        }

        private void OnPathRuleChanged()
        {
            SubItems.Clear();
            Text = PathRule.SourcePathPattern;
            SubItems.Add(PathRule.IsBilingual ? PathRule.TargetPathPattern : PluginResources.SameAsSource);
        }
    }
}