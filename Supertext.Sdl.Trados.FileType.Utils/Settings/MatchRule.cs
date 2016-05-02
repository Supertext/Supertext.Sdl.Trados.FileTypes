using System.ComponentModel;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.Utils.Settings
{
    public class MatchRule : ISerializableListItem, INotifyPropertyChanged
    {
        private const string SettingSegmentionHint = "SegmentationHint";
        private const string SettingTagType = "TagType";
        private const string SettingStartTagRegex = "StartTagRegex";
        private const string SettingEndTagRegex = "EndTagRegex";
        private const string SettingContentTranslatable = "ContentTranslatable";
        private const string SettingWordStop = "WordStop";
        private const string SettingSoftBreak = "SoftBreak";
        private const string SettingCanHide = "CanHide";
        private const string SettingTextEquivalent = "TextEquivalent";
        private const string SettingFormatting = "Formatting";
        private const SegmentationHint DefaultSegmentationHint = SegmentationHint.Undefined;
        private const TagTypeOption DefaultTagType = TagTypeOption.Placeholder;
        private const bool DefaultContentTranslatable = false;
        private const bool DefaultWordStop = false;
        private const bool DefaultSoftBreak = false;
        private const bool DefaultCanHide = false;

        private SegmentationHint _segmentationHint;
        private TagTypeOption _tagType;
        private string _startTagRegex;
        private string _endTagRegex;
        private bool _isContentTranslatable;
        private bool _isWordStop;
        private bool _isSoftBreak;
        private bool _canHide;
        private string _textEquivalent;
        private FormattingGroupSettings _formatting;

        private readonly string _defaultStartTagRegex = string.Empty;
        private readonly string _defaultEndTagRegex = string.Empty;
        private readonly string _defaultTextEquivalent = string.Empty;

        public MatchRule()
        {
            ResetToDefaults();
        }

        public enum TagTypeOption
        {
            Placeholder,
            TagPair
        }

        public SegmentationHint SegmentationHint
        {
            get
            {
                return _segmentationHint;
            }
            set
            {
                _segmentationHint = value;
                OnPropertyChanged("SegmentationHint");
            }
        }

        public TagTypeOption TagType
        {
            get
            {
                return _tagType;
            }
            set
            {
                _tagType = value;
                OnPropertyChanged("TagType");
            }
        }

        public string StartTagRegexValue
        {
            get
            {
                return _startTagRegex;
            }
            set
            {
                _startTagRegex = value;
                OnPropertyChanged("StartTagRegexValue");

            }
        }

        public string EndTagRegexValue
        {
            get
            {
                return _endTagRegex;
            }
            set
            {
                _endTagRegex = value;
                OnPropertyChanged("EndTagRegexValue");
            }
        }

        public bool IsContentTranslatable
        {
            get
            {
                return _isContentTranslatable;
            }
            set
            {
                _isContentTranslatable = value;
                OnPropertyChanged("IsContentTranslatable");
            }
        }

        public bool IsWordStop
        {
            get
            {
                return _isWordStop;
            }
            set
            {
                _isWordStop = value;
                OnPropertyChanged("IsWordStop");
            }
        }

        public bool IsSoftBreak
        {
            get
            {
                return _isSoftBreak;
            }
            set
            {
                _isSoftBreak = value;
                OnPropertyChanged("IsSoftBreak");
            }
        }

        public bool CanHide
        {
            get
            {
                return _canHide;
            }
            set
            {
                _canHide = value;
                OnPropertyChanged("CanHide");
            }
        }

        public string TextEquivalent
        {
            get
            {
                return _textEquivalent;
            }
            set
            {
                _textEquivalent = value;
                OnPropertyChanged("TextEquivalent");
            }
        }

        public FormattingGroupSettings Formatting
        {
            get
            {
                return _formatting;
            }
            set
            {
                _formatting = value;
                OnPropertyChanged("Formatting");
            }
        }

        public void ResetToDefaults()
        {
            SegmentationHint = DefaultSegmentationHint;
            TagType = DefaultTagType;
            StartTagRegexValue = _defaultStartTagRegex;
            EndTagRegexValue = _defaultEndTagRegex;
            IsContentTranslatable = DefaultContentTranslatable;
            IsWordStop = DefaultWordStop;
            IsSoftBreak = DefaultSoftBreak;
            CanHide = DefaultCanHide;
            TextEquivalent = _defaultTextEquivalent;
            Formatting = new FormattingGroupSettings();
        }

        #region ISerializableListItem Members

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + SettingSegmentionHint);
            settingsGroup.RemoveSetting(listItemSetting + SettingTagType);
            settingsGroup.RemoveSetting(listItemSetting + SettingStartTagRegex);
            settingsGroup.RemoveSetting(listItemSetting + SettingEndTagRegex);
            settingsGroup.RemoveSetting(listItemSetting + SettingContentTranslatable);
            settingsGroup.RemoveSetting(listItemSetting + SettingWordStop);
            settingsGroup.RemoveSetting(listItemSetting + SettingSoftBreak);
            settingsGroup.RemoveSetting(listItemSetting + SettingCanHide);
            settingsGroup.RemoveSetting(listItemSetting + SettingTextEquivalent);
            var formattingSettings = new FormattingGroupSettings();
            formattingSettings.ClearListItemSettings(settingsGroup, listItemSetting + SettingFormatting);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            SegmentationHint = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingSegmentionHint, DefaultSegmentationHint);
            TagType = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingTagType, DefaultTagType);
            StartTagRegexValue = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingStartTagRegex, _defaultStartTagRegex);
            EndTagRegexValue = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingEndTagRegex, _defaultEndTagRegex);
            IsContentTranslatable = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingContentTranslatable, DefaultContentTranslatable);
            IsWordStop = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingWordStop, DefaultWordStop);
            IsSoftBreak = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingSoftBreak, DefaultSoftBreak);
            CanHide = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingCanHide, DefaultCanHide);
            TextEquivalent = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingTextEquivalent, _defaultTextEquivalent);
            if (settingsGroup.ContainsSetting(listItemSetting + SettingFormatting))
            {
                _formatting = new FormattingGroupSettings();
                _formatting.PopulateFromSettingsGroup(settingsGroup, listItemSetting + SettingFormatting);
            }
        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingSegmentionHint, SegmentationHint, DefaultSegmentationHint);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingTagType, TagType, DefaultTagType);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingStartTagRegex, StartTagRegexValue, _defaultStartTagRegex);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingEndTagRegex, EndTagRegexValue, _defaultEndTagRegex);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingContentTranslatable, IsContentTranslatable, DefaultContentTranslatable);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingWordStop, IsWordStop, DefaultWordStop);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingSoftBreak, IsSoftBreak, DefaultSoftBreak);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingCanHide, CanHide, DefaultCanHide);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingTextEquivalent, TextEquivalent, _defaultTextEquivalent);
            if (Formatting != null && Formatting.FormattingItems.Count > 0)
            {
                settingsGroup.GetSetting<bool>(listItemSetting + SettingFormatting).Value = true;

                _formatting.SaveToSettingsGroup(settingsGroup, listItemSetting + SettingFormatting);

            }

        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        protected T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
            {
                return settingsGroup.GetSetting<T>(settingName).Value;
            }

            return defaultValue;
        }


        protected void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
        {
            if (settingsGroup == null)
            {
                return;
            }
            if (settingsGroup.ContainsSetting(settingName))
            {
                settingsGroup.GetSetting<T>(settingName).Value = settingValue;
            }
            else
            {
                if (settingValue == null)
                {
                    if ((defaultValue != null))
                    {
                        settingsGroup.GetSetting<T>(settingName).Value = default(T);
                    }
                }
                else
                    if (!settingValue.Equals(defaultValue))
                    {
                        settingsGroup.GetSetting<T>(settingName).Value = settingValue;
                    }
            }

        }
    }
}
