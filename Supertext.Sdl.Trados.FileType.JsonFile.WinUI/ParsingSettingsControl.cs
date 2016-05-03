using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class ParsingSettingsControl : UserControl, IFileTypeSettingsAware<ParsingSettings>
    {
        private ParsingSettings _settings;

        public ParsingSettingsControl()
        {
            InitializeComponent();
        }

        public ParsingSettings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                UpdateUi();
            }
        }

        public void UpdateSettings()
        {
            Settings.IsPathFilteringEnabled = _enablePathFilter.Checked;

            Settings.PathPatterns.Clear();
            Settings.PathPatterns.AddRange(GetPathPatterns());
        }

        public void UpdateUi()
        {
            _enablePathFilter.Checked = Settings.IsPathFilteringEnabled;
            _rulesListView.Items.Clear();
            foreach (var pathPattern in Settings.PathPatterns)
            {
                _rulesListView.Items.Add(new ListViewItem(pathPattern));
            }

            UpdateEnabledState();
        }

        public void UpdateEnabledState()
        {
            var enabled = _enablePathFilter.Checked;

            _rulesListView.Enabled = enabled;
            _addRuleButton.Enabled = enabled;
            _editRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeAllButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
        }

        private IEnumerable<string> GetPathPatterns()
        {
            return _rulesListView.Items.Cast<ListViewItem>().Select(ruleListViewItem => ruleListViewItem.Text).ToList();
        }

        private void _rulesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        private void _enablePathFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (_enablePathFilter.Checked == false)
            {
                _rulesListView.SelectedItems.Clear();
            }

            UpdateEnabledState();
        }

        private void _addRuleButton_Click(object sender, EventArgs e)
        {
            using (var pathRuleForm = new PathRuleForm(string.Empty))
            {
                if (pathRuleForm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _rulesListView.Items.Add(new ListViewItem(pathRuleForm.PathPattern));

                UpdateEnabledState();
            }
        }

        private void _editRuleButton_Click(object sender, EventArgs e)
        {
            if (_rulesListView.SelectedItems.Count <= 0)
            {
                return;
            }

            var ruleListViewItem = _rulesListView.SelectedItems[0];

            if (ruleListViewItem == null)
            {
                return;
            }

            var rule = ruleListViewItem.Text;

            using (var pathRuleForm = new PathRuleForm(rule))
            {
                if (pathRuleForm.ShowDialog(this) == DialogResult.OK)
                {
                    ruleListViewItem.Text = pathRuleForm.PathPattern;
                }
            }
        }

        private void _removeRuleButton_Click(object sender, EventArgs e)
        {
            if (_rulesListView.SelectedItems.Count <= 0)
            {
                return;
            }

            var item = _rulesListView.SelectedItems[0];

            if (item == null)
            {
                return;
            }

            _rulesListView.Items.Remove(item);

            UpdateEnabledState();
        }

        private void _removeAllButton_Click(object sender, EventArgs e)
        {
            _rulesListView.Items.Clear();

            UpdateEnabledState();
        }
    }
}
