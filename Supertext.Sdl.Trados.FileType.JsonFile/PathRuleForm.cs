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
            _ignoreCaseCheckBox.Checked = _pathRule.IgnoreCase;
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            _pathRule.SourcePathPattern = _sourcePathPatternTextBox.Text;
            _pathRule.IgnoreCase = _ignoreCaseCheckBox.Checked;
        }

        private void OnButtonSwap_Click(object sender, EventArgs e)
        {
            var sourcePathLabelCellPosition = _pathRulesTable.GetCellPosition(_sourcePathLabel);
            var targetPathLabelCellPosition = _pathRulesTable.GetCellPosition(_targetPathLabel);
            var sourcePathPatternTextBoxCellPosition = _pathRulesTable.GetCellPosition(_sourcePathPatternTextBox);
            var targetPathPatternCellPosition = _pathRulesTable.GetCellPosition(_targetPathPatternTextBox);

            _pathRulesTable.SetCellPosition(_sourcePathLabel, targetPathLabelCellPosition);
            _pathRulesTable.SetCellPosition(_targetPathLabel, sourcePathLabelCellPosition);
            _pathRulesTable.SetCellPosition(_sourcePathPatternTextBox, targetPathPatternCellPosition);
            _pathRulesTable.SetCellPosition(_targetPathPatternTextBox, sourcePathPatternTextBoxCellPosition);
        }
    }
}