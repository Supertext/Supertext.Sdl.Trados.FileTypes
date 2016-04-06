using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public sealed class EmbeddedContentRegexSettings: FileTypeSettingsBase
    {
        private const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        private const string SettingStructureInfoList = "StructureInfoList";
        private const string SettingMatchRuleList = "MatchRuleList";

        private bool _isEnabled;
        private ObservableList<string> _structureInfos;
        private ComplexObservableList<MatchRule> _matchRules;

        private bool DefaultRegexEmbeddingEnabled = false;

        public EmbeddedContentRegexSettings()
        {
            ResetToDefaults();
        }

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public ObservableList<string> StructureInfos
        {
            get
            {
                return _structureInfos;
            }
            set
            {
                _structureInfos = value;
                OnPropertyChanged("StructureInfos");
            }
        }

        public ComplexObservableList<MatchRule> MatchRules
        {
            get
            {
                return _matchRules;
            }
            set
            {
                _matchRules = value;
                OnPropertyChanged("MatchRules");
            }
        }

        public override void ResetToDefaults()
        {
            Enabled = DefaultRegexEmbeddingEnabled;
            StructureInfos = new ObservableList<string>();
            MatchRules = new ComplexObservableList<MatchRule>();
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            Enabled = GetSettingFromSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled, DefaultRegexEmbeddingEnabled);

            if (settingsGroup.ContainsSetting(SettingStructureInfoList))
            {
                _structureInfos.Clear();
                _structureInfos.PopulateFromSettingsGroup(settingsGroup, SettingStructureInfoList);
            }

            if (settingsGroup.ContainsSetting(SettingMatchRuleList))
            {
                _matchRules.Clear();
                _matchRules.PopulateFromSettingsGroup(settingsGroup, SettingMatchRuleList);
            }
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled, Enabled, DefaultRegexEmbeddingEnabled);
            _structureInfos.SaveToSettingsGroup(settingsGroup, SettingStructureInfoList);
            _matchRules.SaveToSettingsGroup(settingsGroup, SettingMatchRuleList);
        }
    }
}
