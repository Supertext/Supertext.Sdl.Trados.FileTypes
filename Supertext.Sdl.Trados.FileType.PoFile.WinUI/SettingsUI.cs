﻿using System;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;

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
            cb_MessageStringAsSource.Checked = _userSettings.SourceLineType == LineType.MessageString;
            cb_TargetTextNeeded.Checked = _userSettings.IsTargetTextNeeded;
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
            _userSettings.SourceLineType = cb_MessageStringAsSource.Checked
                ? LineType.MessageString
                : LineType.MessageId;
        }

        private void cb_TargetTextNeeded_CheckedChanged(object sender, EventArgs e)
        {
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