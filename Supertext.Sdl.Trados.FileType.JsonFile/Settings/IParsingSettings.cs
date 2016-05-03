using System.Collections.Generic;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public interface IParsingSettings
    {
        bool IsPathFilteringEnabled { get; }

        ComplexObservableList<PathRule> PathRules { get; }

        void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
    }
}
