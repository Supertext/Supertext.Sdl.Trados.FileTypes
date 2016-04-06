﻿using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public interface IEmbeddedContentRegexSettings
    {
        bool Enabled { get; set; }

        ObservableList<string> StructureInfos { get; set; }

        ComplexObservableList<MatchRule> MatchRules { get; set; }

        void ResetToDefaults();

        void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);

        void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
    }
}