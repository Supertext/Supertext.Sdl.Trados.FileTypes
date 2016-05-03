using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class ParsingSettingsControl : UserControl, IFileTypeSettingsAware<ParsingSettings>
    {
        public ParsingSettingsControl()
        {
            InitializeComponent();
        }

        public ParsingSettings Settings { get; set; }

        public void UpdateSettings()
        {
            /*Settings.IsPathFilteringEnabled = _enablePathFilter.Checked;

            Settings.PathPatterns.Clear();
            Settings.PathPatterns.AddRange(GetMatchRules());*/
        }

        public void UpdateUi()
        {
           /* _enablePathFilter.Checked = Settings.IsEnabled;

            _rulesListView.Items.Clear();

            foreach (MatchRule rule in Settings.MatchRules)
            {
                _rulesListView.Items.Add(new RegexRuleListItem(rule));
            }

            UpdateEnabledState();*/
        }
    }
}
