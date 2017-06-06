using System;
using System.ComponentModel;
using System.Windows.Forms;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Views
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
            _targetPathPatternTextBox.Text = _pathRule.IsBilingual ? _pathRule.TargetPathPattern : PluginResources.SameAsSource;
            _ignoreCaseCheckBox.Checked = _pathRule.IgnoreCase;
            _isBilingualCheckBox.Checked = _pathRule.IsBilingual;
            _isTargetValueNeededCheckBox.Checked = _pathRule.IsTargetValueNeeded;

            UpdateTargetInputState();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            if (string.IsNullOrEmpty(_sourcePathPatternTextBox.Text))
            {
                DialogResult = DialogResult.Ignore;
                return;
            }

            _pathRule.SourcePathPattern = _sourcePathPatternTextBox.Text;

            var isBilingual = _isBilingualCheckBox.Checked && !string.IsNullOrEmpty(_targetPathPatternTextBox.Text);

            if (isBilingual)
            {
                _pathRule.TargetPathPattern = _targetPathPatternTextBox.Text;
            }

            _pathRule.IgnoreCase = _ignoreCaseCheckBox.Checked;
            _pathRule.IsBilingual = isBilingual;
            _pathRule.IsTargetValueNeeded = _isTargetValueNeededCheckBox.Checked;
        }

        private void OnIsBilingualCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            _pathRule.IsBilingual = _isBilingualCheckBox.Checked;

            UpdateTargetInputState();
        }

        private void UpdateTargetInputState()
        {
            _targetPathPatternTextBox.Enabled = _isBilingualCheckBox.Checked;

            if (_isBilingualCheckBox.Checked)
            {
                _swapButton.Show();
                _isTargetValueNeededCheckBox.Show();
                _targetPathPatternTextBox.Text = _pathRule.TargetPathPattern;
            }
            else
            {
                _swapButton.Hide();
                _isTargetValueNeededCheckBox.Hide();
                _targetPathPatternTextBox.Text = PluginResources.SameAsSource;
            }
        }

        private void OnSwapButtonClick(object sender, EventArgs e)
        {
            var sourceValue = _sourcePathPatternTextBox.Text;
            _sourcePathPatternTextBox.Text = _targetPathPatternTextBox.Text;
            _targetPathPatternTextBox.Text = sourceValue;
        }
    }
}