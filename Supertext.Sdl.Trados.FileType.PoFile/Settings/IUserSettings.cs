using Sdl.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public interface IUserSettings
    {
        LineType SourceLineType { get; }

        bool IsTargetTextNeeded { get; }

        void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
    }
}