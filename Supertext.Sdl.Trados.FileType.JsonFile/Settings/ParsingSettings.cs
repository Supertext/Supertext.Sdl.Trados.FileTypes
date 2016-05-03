using System.Collections.Generic;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public class ParsingSettings : FileTypeSettingsBase, IParsingSettings
    {
        private const string IsPathFilteringEnabledSetting = "IsPathFilteringEnabled";
        private const string PathPatternsSetting = "PathPatterns";
        private const bool DefaultIsPathFilteringEnabled = false;
        private static readonly List<string> DefaultPathPatterns = new List<string>();

        private bool _isPathFilteringEnabled;
        private List<string> _pathPatterns;

        public ParsingSettings()
        {
            ResetToDefaults();
        }

        public bool IsPathFilteringEnabled
        {
            get { return _isPathFilteringEnabled; }
            set
            {
                _isPathFilteringEnabled = value; 
                OnPropertyChanged("IsPathFilteringEnabled");
            }
        }

        public List<string> PathPatterns
        {
            get { return _pathPatterns; }
            set
            {
                _pathPatterns = value;
                OnPropertyChanged("PathPatterns");
            }
        }

        public sealed override void ResetToDefaults()
        {
            IsPathFilteringEnabled = DefaultIsPathFilteringEnabled;
            PathPatterns = DefaultPathPatterns;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            IsPathFilteringEnabled = GetSettingFromSettingsGroup(settingsGroup, IsPathFilteringEnabledSetting, DefaultIsPathFilteringEnabled);
            PathPatterns = GetSettingFromSettingsGroup(settingsGroup, PathPatternsSetting, DefaultPathPatterns);
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, IsPathFilteringEnabledSetting, IsPathFilteringEnabled, DefaultIsPathFilteringEnabled);
            UpdateSettingInSettingsGroup(settingsGroup, PathPatternsSetting, PathPatterns, DefaultPathPatterns);
        }
    }
}