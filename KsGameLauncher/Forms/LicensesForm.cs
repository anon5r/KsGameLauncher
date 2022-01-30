using System;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class LicensesForm : Form
    {
        public LicensesForm()
        {
            InitializeComponent();
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Licenses_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.appIcon;
            Text = Properties.Strings.LicenseWindowTitle;
            textBox_LicenseText.Text = Properties.Resources.Licenses;
            button_Close.Text = Properties.Strings.ButtonClose;
        }
    }
}
