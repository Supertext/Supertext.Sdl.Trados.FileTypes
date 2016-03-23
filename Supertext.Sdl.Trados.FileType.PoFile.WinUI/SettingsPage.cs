using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    [FileTypeSettingsPage(Id = "PoFile_Settings", Name = "Segment settings", Description = "Segment settings")]
    public class SettingsPage : AbstractFileTypeSettingsPage<SettingsUI, UserSettings>
    {
        /// <summary>
        /// Triggered, when the user clicks the button Reset to Defaults button in 
        /// SDL Trados Studio. Restores the default check box state, which should
        /// be Checked (i.e. product code strings should be locked).
        /// </summary>
        #region "ResetToDefaults"
        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            Control.UpdateControl();
        }
        #endregion

        /// <summary>
        /// Triggered when the user raises the plug-in UI, whose controls (in this case the check box
        /// for locking product code strings) will then be set according to the values stored in 
        /// the settings bundle.
        /// </summary>
        /// <param name="settingsBundle"></param>
        #region "Refresh"
        public override void Refresh()
        {
            base.Refresh();
            Control.UpdateControl();
        }
        #endregion
    }
}
