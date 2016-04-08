using System;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    /// <summary>
    /// Implements the user interface for the file type definition.
    /// </summary>

    #region "ClassDeclaration"
    public partial class SegmentSettingsUI : UserControl, IFileTypeSettingsAware<SegmentSettings>

        #endregion

    {
        /// <summary>
        /// CreateVisitor a settings object based on the SegmentSettings class. 
        /// </summary>

        #region "SettingsObject"
        private SegmentSettings _segmentSettings;

        #endregion

        /// <summary>
        /// Initalize the user interface control by setting it to the
        /// setting value stored in the settings bundle.
        /// </summary>

        #region "Initialize"
        public SegmentSettingsUI()
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
            var isMessageStringSource = _segmentSettings.SourceLineType == LineType.MessageString;
            cb_MessageStringAsSource.Checked = isMessageStringSource;
            cb_TargetTextNeeded.Checked = _segmentSettings.IsTargetTextNeeded;
            tb_sourceSegment.Text = isMessageStringSource ? PluginResources.Example_Msgstr_Value : PluginResources.Example_Msgid_Value;
            tb_targetSegment.Text = _segmentSettings.IsTargetTextNeeded ? PluginResources.Example_Msgstr_Value : string.Empty;
            tb_example.Text = string.Format(PluginResources.Entry_Example_Msgid, PluginResources.Example_Msgid_Value);
            tb_example.Text += Environment.NewLine;
            tb_example.Text += string.Format(PluginResources.Entry_Example_Msgstr, PluginResources.Example_Msgstr_Value);
            tb_note.Text = PluginResources.Segment_Settings_Note;
        }

        #endregion

        /// <summary>
        /// Save the settings based on the value of the the check box.
        /// The setting is saved through the SegmentSettings class, which
        /// handles the plug-in settings bundle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region "SaveSetting"
        private void cb_MessageStringAsSource_CheckedChanged(object sender, EventArgs e)
        {
            tb_sourceSegment.Text = cb_MessageStringAsSource.Checked ? PluginResources.Example_Msgstr_Value : PluginResources.Example_Msgid_Value;

            _segmentSettings.SourceLineType = cb_MessageStringAsSource.Checked
                ? LineType.MessageString
                : LineType.MessageId;
        }

        private void cb_TargetTextNeeded_CheckedChanged(object sender, EventArgs e)
        {
            tb_targetSegment.Text = cb_TargetTextNeeded.Checked ? PluginResources.Example_Msgstr_Value : string.Empty;

            _segmentSettings.IsTargetTextNeeded = cb_TargetTextNeeded.Checked;
        }
        #endregion

        /// <summary>
        /// Implementation of <code>IFileTypeSettingsAware</code> allowing the Filter Framework
        /// to pass through the user settings so that we can initialize the UI.
        /// </summary>

        #region "ApplySettings"
        public SegmentSettings Settings
        {
            get { return _segmentSettings; }
            set
            {
                _segmentSettings = value;
                UpdateControl();
            }
        }

        #endregion
    }
}