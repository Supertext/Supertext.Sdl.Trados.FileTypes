using System.ComponentModel;
using System.Windows.Forms;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class PathRuleForm : Form
    {
        private readonly string _originalPathPattern;

        public PathRuleForm(string pathPattern)
        {
            InitializeComponent();

            _originalPathPattern = pathPattern;
            _pathPatternTextBox.Text = pathPattern;
            PathPattern = pathPattern;
        }

        public string PathPattern { get; private set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                PathPattern = _originalPathPattern;
                return;
            }

            PathPattern = _pathPatternTextBox.Text;
        }
    }
}