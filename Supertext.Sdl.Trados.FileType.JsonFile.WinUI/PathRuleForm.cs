using System.ComponentModel;
using System.Windows.Forms;

namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    public partial class PathRuleForm : Form
    {
        private readonly string _originalRule;

        public PathRuleForm(string rule)
        {
            InitializeComponent();

            _originalRule = rule;
            _pathRuleTextBox.Text = rule;
            Rule = rule;
        }

        public string Rule { get; private set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                Rule = _originalRule;
                return;
            }

            Rule = _pathRuleTextBox.Text;
        }
    }
}