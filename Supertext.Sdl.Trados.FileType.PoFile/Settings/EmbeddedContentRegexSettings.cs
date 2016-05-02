using System.ComponentModel;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public sealed class EmbeddedContentRegexSettings : FileTypeSettingsBase, IEmbeddedContentRegexSettings
    {
        private const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        private const string SettingStructureInfoList = "StructureInfoList";
        private const string SettingMatchRuleList = "MatchRuleList";
        private const bool DefaultRegexEmbeddingEnabled = true;
        private static readonly ObservableList<string> DefaultStructureInfos =  new ObservableList<string>
        {
            "sdl:field"
        };
        public static ComplexObservableList<MatchRule> DefaultMatchRules = new ComplexObservableList<MatchRule>
        {
            new MatchRule
            {
                CanHide = false,
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"\$\[\w+\]",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                CanHide = false,
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"%\w+",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                CanHide = false,
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"\$\w+",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                CanHide = false,
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*[^<>]*\s*\/>",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                CanHide = false,
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*([^<>\/]|\"".*\"")*>",
                EndTagRegexValue = @"<\/[a-zA-Z][a-zA-Z0-9]*[^<>]*>",
                SegmentationHint = SegmentationHint.IncludeWithText,
                IsContentTranslatable = true
            }
        };

        private bool _isIsEnabled;
        private ObservableList<string> _structureInfos;
        private ComplexObservableList<MatchRule> _matchRules;

        public EmbeddedContentRegexSettings()
        {
            ResetToDefaults();
        }

        public bool IsEnabled
        {
            get { return _isIsEnabled; }
            set
            {
                _isIsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public ObservableList<string> StructureInfos
        {
            get { return _structureInfos; }
            set
            {
                _structureInfos = value;
                OnPropertyChanged("StructureInfos");
            }
        }

        public ComplexObservableList<MatchRule> MatchRules
        {
            get { return _matchRules; }
            set
            {
                _matchRules = value;
                OnPropertyChanged("MatchRules");
            }
        }

        public override void ResetToDefaults()
        {
            IsEnabled = DefaultRegexEmbeddingEnabled;            
            StructureInfos = DefaultStructureInfos;
            MatchRules = DefaultMatchRules;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            IsEnabled = GetSettingFromSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled,
                DefaultRegexEmbeddingEnabled);

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
            UpdateSettingInSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled, IsEnabled,
                DefaultRegexEmbeddingEnabled);
            _structureInfos.SaveToSettingsGroup(settingsGroup, SettingStructureInfoList);
            _matchRules.SaveToSettingsGroup(settingsGroup, SettingMatchRuleList);
        }
    }
}