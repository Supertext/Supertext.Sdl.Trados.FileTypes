﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile
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
            get { return _settings; }
            set
            {
                _settings = value;
                UpdateUi();
            }
        }

        public void UpdateSettings()
        {
            Settings.IsPathFilteringEnabled = _enablePathFilter.Checked;

            Settings.PathRules.Clear();
            Settings.PathRules.AddRange(GetPathRules());
        }

        public void UpdateUi()
        {
            _enablePathFilter.Checked = Settings.IsPathFilteringEnabled;
            _rulesListView.Items.Clear();

            foreach (var pathRule in Settings.PathRules)
            {
                _rulesListView.Items.Add(new PathRuleListViewItem(pathRule));
            }

            UpdateEnabledState();
        }

        public void UpdateEnabledState()
        {
            var enabled = _enablePathFilter.Checked;

            _rulesListView.Enabled = enabled;
            _addRuleButton.Enabled = enabled;
            _extractButton.Enabled = enabled;
            _editRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeAllButton.Enabled = enabled;
        }

        private IEnumerable<PathRule> GetPathRules()
        {
            return
                _rulesListView.Items.Cast<PathRuleListViewItem>()
                    .Select(pathRuleListViewItem => pathRuleListViewItem.PathRule)
                    .ToList();
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
            var pathRule = new PathRule();

            using (var pathRuleForm = new PathRuleForm(pathRule))
            {
                if (pathRuleForm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _rulesListView.Items.Add(new PathRuleListViewItem(pathRule));

                UpdateEnabledState();
            }
        }

        private void _editRuleButton_Click(object sender, EventArgs e)
        {
            foreach (PathRuleListViewItem ruleListViewItem in _rulesListView.SelectedItems)
            {
                if (ruleListViewItem == null)
                {
                    continue;
                }

                var pathRule = ruleListViewItem.PathRule;

                using (var pathRuleForm = new PathRuleForm(pathRule))
                {
                    if (pathRuleForm.ShowDialog(this) == DialogResult.OK)
                    {
                        ruleListViewItem.PathRule = pathRule;
                    }
                }
            }
        }

        private void _removeRuleButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in _rulesListView.SelectedItems)
            {
                if (item == null)
                {
                    continue;
                }

                _rulesListView.Items.Remove(item);
            }

            UpdateEnabledState();
        }

        private void _removeAllButton_Click(object sender, EventArgs e)
        {
            _rulesListView.Items.Clear();

            UpdateEnabledState();
        }

        private void _helpButton_Click(object sender, EventArgs e)
        {
            using (var pathRuleHelpForm = new PathRuleHelpForm())
            {
                pathRuleHelpForm.ShowDialog(this);
            }
        }

        private void _extractButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = PluginResources.FileTypeFilter;
                openFileDialog.Multiselect = true;

                var dialogResult = openFileDialog.ShowDialog();

                if (dialogResult != DialogResult.OK)
                {
                    return;
                }

                ExtractFiles(openFileDialog.FileNames);
            }
        }

        private void ExtractFiles(string[] files)
        {
            var jsonPathPatternExtractor = new JsonPathPatternExtractor(new JsonFactory());

            foreach (var file in files)
            {
                var existingPathPatterns = _rulesListView.Items.Cast<PathRuleListViewItem>()
                .Select(pathRuleListViewItem => pathRuleListViewItem.PathRule.SourcePathPattern).ToList();

                var extractedPathPatterns = jsonPathPatternExtractor.ExtractPathPatterns(file);

                foreach (var pathPattern in extractedPathPatterns.Except(existingPathPatterns))
                {
                    _rulesListView.Items.Add(new PathRuleListViewItem(new PathRule { SourcePathPattern = pathPattern }));
                }
            }
        }
    }
}