using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    [FileTypeSettingsPage(Id = "Community_Embeddded_Content_Processor_Settings", Name = "Community_Embedded_Content_Processor_Settings_Name",
       Description = "Community_Embedded_Content_Processor_Settings_Description", HelpTopic = "Embedded_Regex_Content")]
    public class EmbeddedContentSettingsPage : AbstractFileTypeSettingsPage<EmbeddedContentSettingsControl, EmbeddedContentRegexSettings>
    {
        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            Control.UpdateUi();
        }

        public override void Save()
        {
            Control.UpdateSettings();
            base.Save();
        }

        public override void Refresh()
        {
            base.Refresh();
            Control.UpdateUi();
        }
    }
}
