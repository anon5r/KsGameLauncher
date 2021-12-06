using System;
using System.Windows.Forms;

namespace KonaStaGameLauncher
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.app48;
        }

        private void About_Load(object sender, EventArgs e)
        {
            Text = Resources.AboutThisApp;
            label_Application.Text = Resources.AppName;
            label_Version.Text = "ver. " + Application.ProductVersion;
            label_Authors.Text = Properties.Resources.Authors;
            linkLabel_Support.Text = Properties.Resources.SupportLabelText;
            linkLabel_License.Text = Resources.ShowLicense;

            button_Ok.Focus();
        }

        private void LinkLabel_Support_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.SupportLabelURL);
        }

        private void Button_Ok_Click(object sender, EventArgs e) => Close();

        private void LinkLabel_License_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            (new LicensesForm()).Show(this);
        }
    }
}
