using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
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
            Text = _pathRule.PathPattern;
        }
    }
}