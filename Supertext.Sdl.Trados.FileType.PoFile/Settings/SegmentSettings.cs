﻿using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Settings
{
    public sealed class SegmentSettings : FileTypeSettingsBase, ISegmentSettings
    {
        private const string SourceLineTypeSetting = "SourceLineType";
        private const string IsTargetTextNeededSetting = "IsTargetTextNeeded";
        private const LineType DefaultSourceLineType = LineType.MessageId;
        private const bool DefaultIsTargetTextNeededSetting = true;
        private LineType _sourceLineType;
        private bool _isTargetTextNeeded;

        public SegmentSettings()
        {
            ResetToDefaults();
        }

        public LineType SourceLineType
        {
            get { return _sourceLineType; }
            set
            {
                _sourceLineType = value;
                OnPropertyChanged("SourceLineType");
            }
        }

        public bool IsTargetTextNeeded
        {
            get { return _isTargetTextNeeded; }
            set
            {
                _isTargetTextNeeded = value;
                OnPropertyChanged("IsTargetTextNeeded");
            }
        }

        public override void ResetToDefaults()
        {
            SourceLineType = DefaultSourceLineType;
            IsTargetTextNeeded = DefaultIsTargetTextNeededSetting;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            SourceLineType = GetSettingFromSettingsGroup(settingsGroup, SourceLineTypeSetting, DefaultSourceLineType);
            IsTargetTextNeeded = GetSettingFromSettingsGroup(settingsGroup, IsTargetTextNeededSetting, DefaultIsTargetTextNeededSetting);
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, SourceLineTypeSetting, SourceLineType, DefaultSourceLineType);
            UpdateSettingInSettingsGroup(settingsGroup, IsTargetTextNeededSetting, IsTargetTextNeeded, DefaultIsTargetTextNeededSetting);
        }
    }
}