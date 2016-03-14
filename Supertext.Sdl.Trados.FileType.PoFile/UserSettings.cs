using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IUserSettings
    {
        LineType LineTypeToTranslate { get; }

        void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
    }

    public sealed class UserSettings : FileTypeSettingsBase, IUserSettings
    {
        private LineType _lineTypeToTranslate;
        private const string LineTypeToTranslateSetting = "LineTypeToTranslate";
        private const LineType DefaultLineTypeToTranslate = LineType.MessageId;

        public UserSettings()
        {
            ResetToDefaults();
        }

        public LineType LineTypeToTranslate
        {
            get { return _lineTypeToTranslate; }
            set
            {
                _lineTypeToTranslate = value;
                OnPropertyChanged("LineTypeToTranslate");
            }
        }

        public override void ResetToDefaults()
        {
            LineTypeToTranslate = DefaultLineTypeToTranslate;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            LineTypeToTranslate = GetSettingFromSettingsGroup(settingsGroup, LineTypeToTranslateSetting, DefaultLineTypeToTranslate);
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, LineTypeToTranslateSetting, LineTypeToTranslate, DefaultLineTypeToTranslate);
        }
    }
}