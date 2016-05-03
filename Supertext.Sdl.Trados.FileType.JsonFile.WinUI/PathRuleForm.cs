using System;
using System.ComponentModel;
using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class PathRuleForm : Form
    {
        private readonly PathRule _pathRule;

        public PathRuleForm(PathRule pathRule)
        {
            _pathRule = pathRule;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _pathPatternTextBox.Text = _pathRule.PathPattern;
            _ignoreCaseCheckBox.Checked = _pathRule.IgnoreCase;
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            _pathRule.PathPattern = _pathPatternTextBox.Text;
            _pathRule.IgnoreCase = _ignoreCaseCheckBox.Checked;
        }
    }
}