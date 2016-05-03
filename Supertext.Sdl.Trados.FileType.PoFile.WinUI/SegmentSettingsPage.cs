using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    [FileTypeSettingsPage(Id = "PoFile_Segment_Settings", Name = "Segment_Settings_Name", Description = "Segment_Settings_Description")]
    public class SegmentSettingsPage : AbstractFileTypeSettingsPage<SegmentSettingsControl, SegmentSettings>
    {
        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            Control.UpdateUi();
        }

        public override void Refresh()
        {
            base.Refresh();
            Control.UpdateUi();
        }
    }
}
