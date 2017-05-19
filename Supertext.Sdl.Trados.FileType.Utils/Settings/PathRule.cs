using System.ComponentModel;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.Utils.Settings
{
    public class PathRule : ISerializableListItem, INotifyPropertyChanged
    {
        private const string SourcePathPatternSetting = "SourcePathPattern";
        private const string TargetPathPatternSetting = "TargetPathPattern";
        private const string IgnoreCaseSetting = "IgnoreCasePattern";
        private const string IsBilingualSetting = "IsBilingual";
        private const string IsTargetValueNeededSetting = "IsTargetValueNeeded";

        private const bool DefaultIgnoreCase = false;
        private const bool DefaultIsBilingual = false;
        private const bool DefaultIsTargetValueNeeded = false;
        private static readonly string DefaultSourcePathPattern = string.Empty;
        private static readonly string DefaultTargetPathPattern = string.Empty;

        private string _sourcePathPattern;
        private string _targetPathPattern;
        private bool _ignoreCase;
        private bool _isBilingual;
        private bool _isTargetValueNeeded;

        public event PropertyChangedEventHandler PropertyChanged;

        public PathRule()
        {
            ResetToDefaults();
        }

        public string SourcePathPattern
        {
            get { return _sourcePathPattern; }
            set
            {
                _sourcePathPattern = value;
                OnPropertyChanged("SourcePathPattern");
            }
        }

        public string TargetPathPattern
        {
            get { return _targetPathPattern; }
            set
            {
                _targetPathPattern = value;
                OnPropertyChanged("TargetPathPattern");
            }
        }

        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set
            {
                _ignoreCase = value;
                OnPropertyChanged("IgnoreCase");
            }
        }

        public bool IsBilingual {
            get { return _isBilingual; }
            set
            {
                _isBilingual = value;
                OnPropertyChanged("IsBilingual");
            }
        }

        public bool IsTargetValueNeeded
        {
            get { return _isTargetValueNeeded; }
            set
            {
                _isTargetValueNeeded = value;
                OnPropertyChanged("IsTargetValueNeeded");
            }
        }

        public void ResetToDefaults()
        {
            SourcePathPattern = DefaultSourcePathPattern;
            TargetPathPattern = DefaultTargetPathPattern;
            IgnoreCase = DefaultIgnoreCase;
            IsBilingual = DefaultIsBilingual;
            IsTargetValueNeeded = DefaultIsTargetValueNeeded;
        }

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + SourcePathPatternSetting);
            settingsGroup.RemoveSetting(listItemSetting + TargetPathPatternSetting);
            settingsGroup.RemoveSetting(listItemSetting + IgnoreCaseSetting);
            settingsGroup.RemoveSetting(listItemSetting + IsBilingualSetting);
            settingsGroup.RemoveSetting(listItemSetting + IsTargetValueNeededSetting);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            SourcePathPattern = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SourcePathPatternSetting, DefaultSourcePathPattern);
            TargetPathPattern = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + TargetPathPatternSetting, DefaultTargetPathPattern);
            IgnoreCase = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + IgnoreCaseSetting, DefaultIgnoreCase);
            IsBilingual = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + IsBilingualSetting, DefaultIsBilingual);
            IsTargetValueNeeded = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + IsTargetValueNeededSetting, DefaultIsTargetValueNeeded);
        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SourcePathPatternSetting, SourcePathPattern, DefaultSourcePathPattern);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + TargetPathPatternSetting, TargetPathPattern, DefaultTargetPathPattern);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + IgnoreCaseSetting, IgnoreCase, DefaultIgnoreCase);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + IsBilingualSetting, IsBilingual, DefaultIsBilingual);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + IsTargetValueNeededSetting, IsTargetValueNeeded, DefaultIsTargetValueNeeded);
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
