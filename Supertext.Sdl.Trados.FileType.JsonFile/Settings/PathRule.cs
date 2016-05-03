using System.ComponentModel;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public class PathRule : ISerializableListItem, INotifyPropertyChanged
    {
        private const string PathPatternSetting = "PathPattern";

        private static readonly string DefaultPathPattern = string.Empty;
        private string _pathPattern;

        public event PropertyChangedEventHandler PropertyChanged;

        public PathRule()
        {
            ResetToDefaults();
        }


        public string PathPattern
        {
            get { return _pathPattern; }
            set
            {
                _pathPattern = value;
                OnPropertyChanged("PathPattern");
            }
        }

        public void ResetToDefaults()
        {
            PathPattern = DefaultPathPattern;
        }

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + PathPatternSetting);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            PathPattern = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + PathPatternSetting, DefaultPathPattern);
        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + PathPatternSetting, PathPattern, DefaultPathPattern);
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
