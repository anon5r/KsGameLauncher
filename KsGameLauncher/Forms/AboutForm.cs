using System;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.app;
        }

        private void About_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.app;
            Text = Resources.AboutThisApp;
            label_Application.Text = Resources.AppName;
            label_Version.Text = "ver. " + Application.ProductVersion;
            label_Develop.Text = Resources.LabelDeveloper;
            AppDeveloper.Text = Properties.Resources.Developers;
            textBox_SpecialThanks.Text = 
                //string.Format("AppIcon: {0}", Properties.Resources.AppIconDesigner) + 
                Properties.Resources.SpecialThanks;
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
            (new LicensesForm()).ShowDialog(this);
        }

        private void AboutForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
