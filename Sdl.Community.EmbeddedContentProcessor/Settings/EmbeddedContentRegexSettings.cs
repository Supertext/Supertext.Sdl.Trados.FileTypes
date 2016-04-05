using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.EmbeddedContentProcessor.Settings
{
    public class EmbeddedContentRegexSettings: FileTypeSettingsBase
    {
        const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        const string SettingStructureInfoList = "StructureInfoList";
        const string SettingMatchRuleList = "MatchRuleList";

        bool _isEnabled;
        ObservableList<string> _structureInfos;
        ComplexObservableList<MatchRule> _matchRules;

        protected bool DefaultRegexEmbeddingEnabled = false;

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

        public sealed override void ResetToDefaults()
        {
            Enabled = DefaultRegexEmbeddingEnabled;
            StructureInfos = GetDefaultContexts();
            MatchRules = GetDefaultRules();
        }

        private ComplexObservableList<MatchRule> GetDefaultRules()
        {
            return new ComplexObservableList<MatchRule>
            {
                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = "(\\\\{2})?</?[a-z][a-z0-9]*[^<>]*>",
                    SegmentationHint = SegmentationHint.IncludeWithText
                },
                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = "%\\d+",
                    SegmentationHint = SegmentationHint.IncludeWithText
                },
                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = "\\$\\[\\w+\\]",
                    SegmentationHint = SegmentationHint.IncludeWithText
                },
                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = "%\\w+",
                    SegmentationHint = SegmentationHint.IncludeWithText
                },
                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = "\\$\\w+",
                    SegmentationHint = SegmentationHint.IncludeWithText
                }
            };
        }

        private ObservableList<string> GetDefaultContexts()
        {
            return new ObservableList<string>
            {
                StandardContextTypes.Field
            };
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
