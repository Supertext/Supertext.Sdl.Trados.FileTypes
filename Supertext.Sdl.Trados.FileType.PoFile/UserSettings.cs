using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IUserSettings
    {
        LineType SourceLineType { get; }

        void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
    }

    public sealed class UserSettings : FileTypeSettingsBase, IUserSettings
    {
        private LineType _sourceLineType;
        private const string SourceLineTypeSetting = "SourceLineType";
        private const LineType DefaultSourceLineType = LineType.MessageId;

        public UserSettings()
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

        public override void ResetToDefaults()
        {
            SourceLineType = DefaultSourceLineType;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            SourceLineType = GetSettingFromSettingsGroup(settingsGroup, SourceLineTypeSetting, DefaultSourceLineType);
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, SourceLineTypeSetting, SourceLineType, DefaultSourceLineType);
        }
    }
}