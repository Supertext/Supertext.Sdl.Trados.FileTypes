﻿using System.ComponentModel;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public sealed class EmbeddedContentRegexSettings: FileTypeSettingsBase, IEmbeddedContentRegexSettings
    {
        private const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        private const string SettingStructureInfoList = "StructureInfoList";
        private const string SettingMatchRuleList = "MatchRuleList";
        private const bool DefaultRegexEmbeddingEnabled = false;
        public static ComplexObservableList<MatchRule> DefaultMatchRules = new ComplexObservableList<MatchRule>
            {

                new MatchRule
                {
                    CanHide = false,
                    TagType = MatchRule.TagTypeOption.Placeholder,
                    StartTagRegexValue = @"%\d+",
                    SegmentationHint = SegmentationHint.IncludeWithText
                },
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
                    TagType = MatchRule.TagTypeOption.TagPair,
                    StartTagRegexValue =  @"<[a-z][a-z0-9]*[^<>]*>",
                    EndTagRegexValue =  @"<\/[a-z][a-z0-9]*[^<>]*>",
                    SegmentationHint = SegmentationHint.IncludeWithText
                }
            };

        private bool _isEnabled;
        private ObservableList<string> _structureInfos;
        private ComplexObservableList<MatchRule> _matchRules;


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
            StructureInfos = new ObservableList<string>
            {
                "sdl:field"
            };
            MatchRules = DefaultMatchRules;
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
