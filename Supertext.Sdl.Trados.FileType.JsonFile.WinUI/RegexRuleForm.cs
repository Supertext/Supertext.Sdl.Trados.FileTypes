﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class RegexRuleForm : Form
    {
        readonly MatchRule _rule;
        private static readonly string TagPair = PluginResources.Tag_Pair;
        private static readonly string PlaceholderType = PluginResources.Placeholder;
        private static readonly string Translatable = PluginResources.Translatable;
        private static readonly string NotTranslatable = PluginResources.Not_Translatable;
       
        
        public RegexRuleForm(MatchRule rule)
        {
            InitializeComponent();

            _rule = rule;

            _ruleTypeComboBox.Text = rule.TagType == MatchRule.TagTypeOption.TagPair ? TagPair : PlaceholderType;

            _startTagTextBox.Text = _rule.StartTagRegexValue;
            _endTagTextBox.Text = _rule.EndTagRegexValue;
            
            if (rule.IsContentTranslatable)
            {
                _translateComboBox.Text = Translatable;
            }
            else
            {
                _translateComboBox.Text = NotTranslatable;
            }
        }

     
        private void _ruleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ruleTypeComboBox.Text == PlaceholderType)
            {
                l_Closing.Visible = false;
                _endTagTextBox.Visible = false;
                _translateComboBox.Enabled = false;
                _translateComboBox.Text = NotTranslatable;
                _endTagTextBox.Text = "";
            }
            else
            {
                l_Closing.Visible = true;
                _endTagTextBox.Visible = true;
                _translateComboBox.Enabled = true;
                if (_translateComboBox.Items.Count > 0)
                {
                    _translateComboBox.SelectedIndex = 0;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
           if (_ruleTypeComboBox.Items.Count == 0)
            {
                _ruleTypeComboBox.Items.Add(TagPair);
                _ruleTypeComboBox.Items.Add(PlaceholderType);
            }
            
            _ruleTypeComboBox.SelectedItem = _rule.TagType == MatchRule.TagTypeOption.TagPair ? TagPair : PlaceholderType;

            if (_translateComboBox.Items.Count == 0)
            {
                _translateComboBox.Items.Add(Translatable);
                _translateComboBox.Items.Add(NotTranslatable);
            }

            _translateComboBox.SelectedItem = _rule.IsContentTranslatable ? Translatable : NotTranslatable;

            LoadSegmentationHint();
        }

        private void LoadSegmentationHint()
        {
            _segmentationHintComboBox.DisplayMember = "DisplayValue";
            var list = new SegmentationHintComboItems();
            
            foreach (var item in list)
            {
                _segmentationHintComboBox.Items.Add(item);
                if (item.SegmentationHint == _rule.SegmentationHint)
                {
                    _segmentationHintComboBox.SelectedItem = item;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK) return;
            //check the dialog has valid settings
            if ((_ruleTypeComboBox.SelectedItem.ToString() == TagPair &&
                 (_endTagTextBox.Text.Length == 0 || _startTagTextBox.Text.Length == 0)) ||
                (_ruleTypeComboBox.SelectedItem.ToString() == PlaceholderType &&
                 _startTagTextBox.Text.Length == 0))
            {
                MessageBox.Show("The regular expression pattern must be specified.",
                    "Embedded Processing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                e.Cancel = true;
            }

            _rule.StartTagRegexValue = _startTagTextBox.Text;
            _rule.EndTagRegexValue = _endTagTextBox.Text;

            _rule.TagType = _ruleTypeComboBox.SelectedItem.ToString() == TagPair ? MatchRule.TagTypeOption.TagPair : MatchRule.TagTypeOption.Placeholder;

            _rule.SegmentationHint =
                ((SegmentationHintComboItem) _segmentationHintComboBox.SelectedItem).SegmentationHint;

            _rule.IsContentTranslatable = _translateComboBox.SelectedItem.ToString() == Translatable;
        }

    private void tb_Opening_TextChanged(object sender, EventArgs e)
        {
            ValidateRegexTextBox(_startTagTextBox);
        }

        private void tb_Closing_TextChanged(object sender, EventArgs e)
        {
            ValidateRegexTextBox(_endTagTextBox);
        }

        private void ValidateRegexTextBox(TextBox validationBox)
        {
            try
            {
                if(validationBox.Text.Length > 0)
                {
                    var validator = new RegexStringValidator(validationBox.Text);
                }

                _errorProvider.SetError(validationBox, "");
                validationBox.ForeColor = Color.Black;
            }
            catch (ArgumentException e)
            {
                _errorProvider.SetIconAlignment(validationBox, ErrorIconAlignment.MiddleRight);
                _errorProvider.SetIconPadding(validationBox, -20);
                _errorProvider.SetError(validationBox, "The regular expression is incorrect:" + "\n" + e.Message);
                validationBox.ForeColor = Color.Red;
            }
        }
    }

    class SegmentationHintComboItem
    {
        public string DisplayValue { get; set; }

        public SegmentationHint SegmentationHint { get; set; }
    }

    class SegmentationHintComboItems : List<SegmentationHintComboItem>
    {
        public SegmentationHintComboItems()
        {
            Add(new SegmentationHintComboItem { DisplayValue = PluginResources.Include, SegmentationHint = SegmentationHint.Include });
            Add(new SegmentationHintComboItem { DisplayValue = PluginResources.Include_with_text, SegmentationHint = SegmentationHint.IncludeWithText });
            Add(new SegmentationHintComboItem { DisplayValue = PluginResources.Exclude, SegmentationHint = SegmentationHint.Exclude });
            Add(new SegmentationHintComboItem { DisplayValue = PluginResources.May_Exclude, SegmentationHint = SegmentationHint.MayExclude });
            Add(new SegmentationHintComboItem { DisplayValue = PluginResources.Undefined, SegmentationHint = SegmentationHint.Undefined });
        }
    }
}

