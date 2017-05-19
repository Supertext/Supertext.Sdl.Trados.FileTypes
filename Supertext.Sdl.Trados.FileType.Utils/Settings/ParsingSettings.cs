using System.Collections.Generic;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.Utils.Settings
{
    public class ParsingSettings : FileTypeSettingsBase, IParsingSettings
    {
        private const string IsPathFilteringEnabledSetting = "IsPathFilteringEnabled";
        private const string PathRulesSetting = "PathRules";
        private const bool DefaultIsPathFilteringEnabled = false;
        private static readonly ComplexObservableList<PathRule> DefaultPathRules = new ComplexObservableList<PathRule>();

        private bool _isPathFilteringEnabled;
        private ComplexObservableList<PathRule> _pathRules;

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

        public ComplexObservableList<PathRule> PathRules
        {
            get { return _pathRules; }
            set
            {
                _pathRules = value;
                OnPropertyChanged("PathRules");
            }
        }

        public sealed override void ResetToDefaults()
        {
            IsPathFilteringEnabled = DefaultIsPathFilteringEnabled;
            PathRules = DefaultPathRules;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            IsPathFilteringEnabled = GetSettingFromSettingsGroup(settingsGroup, IsPathFilteringEnabledSetting, DefaultIsPathFilteringEnabled);

            if (settingsGroup.ContainsSetting(PathRulesSetting))
            {
                _pathRules.Clear();
                _pathRules.PopulateFromSettingsGroup(settingsGroup, PathRulesSetting);
            }
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, IsPathFilteringEnabledSetting, IsPathFilteringEnabled, DefaultIsPathFilteringEnabled);
            _pathRules.SaveToSettingsGroup(settingsGroup, PathRulesSetting);
        }
    }
}