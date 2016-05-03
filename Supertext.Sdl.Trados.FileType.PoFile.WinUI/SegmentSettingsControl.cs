using System;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    public partial class SegmentSettingsControl : UserControl, IFileTypeSettingsAware<SegmentSettings>
    {
        private const string SourceExample = "Example";
        private const string TargetExample = "Beispiel";

        private SegmentSettings _segmentSettings;

        public SegmentSettings Settings
        {
            get { return _segmentSettings; }
            set
            {
                _segmentSettings = value;
                UpdateUi();
            }
        }

        public SegmentSettingsControl()
        {
            InitializeComponent();
        }
        
        public void UpdateUi()
        {
            var isMessageStringSource = _segmentSettings.SourceLineType == LineType.MessageString;
            cb_MessageStringAsSource.Checked = isMessageStringSource;
            cb_TargetTextNeeded.Checked = _segmentSettings.IsTargetTextNeeded;
            tb_sourceSegment.Text = isMessageStringSource ? TargetExample : SourceExample;
            tb_targetSegment.Text = _segmentSettings.IsTargetTextNeeded ? TargetExample : string.Empty;
        }

        private void cb_MessageStringAsSource_CheckedChanged(object sender, EventArgs e)
        {
            tb_sourceSegment.Text = cb_MessageStringAsSource.Checked ? TargetExample : SourceExample;

            _segmentSettings.SourceLineType = cb_MessageStringAsSource.Checked
                ? LineType.MessageString
                : LineType.MessageId;
        }

        private void cb_TargetTextNeeded_CheckedChanged(object sender, EventArgs e)
        {
            tb_targetSegment.Text = cb_TargetTextNeeded.Checked ? TargetExample : string.Empty;

            _segmentSettings.IsTargetTextNeeded = cb_TargetTextNeeded.Checked;
        }
    }
}