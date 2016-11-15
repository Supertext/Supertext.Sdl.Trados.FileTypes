using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    [FileTypeSettingsPage(Id = "JsonFile_Parsing_Settings", Name = "Parsing_Settings_Name", Description = "Parsing_Settings_Description")]
    public class ParsingSettingsPage : AbstractFileTypeSettingsPage<ParsingSettingsControl, ParsingSettings>
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
