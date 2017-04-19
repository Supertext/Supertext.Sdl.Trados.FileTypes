using System;
using System.ComponentModel;
using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile
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
            _sourcePathPatternTextBox.Text = _pathRule.SourcePathPattern;
            _targetPathPatternTextBox.Text = _pathRule.TargetPathPattern;
            _ignoreCaseCheckBox.Checked = _pathRule.IgnoreCase;
            _isBilingualCheckBox.Checked = _pathRule.IsBilingual;

            UpdateTargetInputState();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            _pathRule.SourcePathPattern = _sourcePathPatternTextBox.Text;
            _pathRule.TargetPathPattern = _targetPathPatternTextBox.Text;
            _pathRule.IgnoreCase = _ignoreCaseCheckBox.Checked;
            _pathRule.IsBilingual = _isBilingualCheckBox.Checked;
        }

        private void OnIsBilingualCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            _pathRule.IsBilingual = _isBilingualCheckBox.Checked;

            UpdateTargetInputState();
        }

        private void UpdateTargetInputState()
        {
            _targetPathPatternTextBox.Enabled = _isBilingualCheckBox.Checked;
            _swapButton.Enabled = _isBilingualCheckBox.Checked;
        }

        private void OnSwapButtonClick(object sender, EventArgs e)
        {
            var sourceValue = _sourcePathPatternTextBox.Text;
            _sourcePathPatternTextBox.Text = _targetPathPatternTextBox.Text;
            _targetPathPatternTextBox.Text = sourceValue;
        }
    }
}