using System;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    /// <summary>
    /// Implements the user interface for the file type definition.
    /// </summary>

    #region "ClassDeclaration"
    public partial class SettingsUI : UserControl, IFileTypeSettingsAware<UserSettings>

        #endregion

    {
        /// <summary>
        /// Create a settings object based on the UserSettings class. 
        /// </summary>

        #region "SettingsObject"
        private UserSettings _userSettings;

        #endregion

        /// <summary>
        /// Initalize the user interface control by setting it to the
        /// setting value stored in the settings bundle.
        /// </summary>

        #region "Initialize"
        public SettingsUI()
        {
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Reset the user interface control to its default value, which is
        /// checked, i.e. the product lock option should be enabled
        /// by default.
        /// </summary>

        #region "UpdateControl"
        public void UpdateControl()
        {
            var isMessageStringSource = _userSettings.SourceLineType == LineType.MessageString;
            cb_MessageStringAsSource.Checked = isMessageStringSource;
            cb_TargetTextNeeded.Checked = _userSettings.IsTargetTextNeeded;
            tb_sourceSegment.Text = isMessageStringSource ? PluginResources.Example_Msgstr_Value : PluginResources.Example_Msgid_Value;
            tb_targetSegment.Text = _userSettings.IsTargetTextNeeded ? PluginResources.Example_Msgstr_Value : string.Empty;
            tb_example.Text = string.Format(PluginResources.Entry_Example_Msgid, PluginResources.Example_Msgid_Value);
            tb_example.Text += Environment.NewLine;
            tb_example.Text += string.Format(PluginResources.Entry_Example_Msgstr, PluginResources.Example_Msgstr_Value);
            tb_note.Text = PluginResources.Segment_Settings_Note;
        }

        #endregion

        /// <summary>
        /// Save the settings based on the value of the the check box.
        /// The setting is saved through the UserSettings class, which
        /// handles the plug-in settings bundle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region "SaveSetting"
        private void cb_MessageStringAsSource_CheckedChanged(object sender, EventArgs e)
        {
            tb_sourceSegment.Text = cb_MessageStringAsSource.Checked ? PluginResources.Example_Msgstr_Value : PluginResources.Example_Msgid_Value;

            _userSettings.SourceLineType = cb_MessageStringAsSource.Checked
                ? LineType.MessageString
                : LineType.MessageId;
        }

        private void cb_TargetTextNeeded_CheckedChanged(object sender, EventArgs e)
        {
            tb_targetSegment.Text = cb_TargetTextNeeded.Checked ? PluginResources.Example_Msgstr_Value : string.Empty;

            _userSettings.IsTargetTextNeeded = cb_TargetTextNeeded.Checked;
        }
        #endregion

        /// <summary>
        /// Implementation of <code>IFileTypeSettingsAware</code> allowing the Filter Framework
        /// to pass through the user settings so that we can initialize the UI.
        /// </summary>

        #region "ApplySettings"
        public UserSettings Settings
        {
            get { return _userSettings; }
            set
            {
                _userSettings = value;
                UpdateControl();
            }
        }

        #endregion
    }
}