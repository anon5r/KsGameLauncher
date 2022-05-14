using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.appIcon;
        }

        private void About_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.appIcon;
            Text = Properties.Strings.AboutThisApp;
            label_Application.Text = Properties.Strings.AppName;
            label_Version.Text = "ver. " + Application.ProductVersion;
            label_Develop.Text = Properties.Strings.LabelDeveloper;
            AppDeveloper.Text = Properties.Resources.Developers;
            textBox_Copyrights.Text = "";
            if (GetInstalledGamesName().Length > 0)
            {
                string installedGameRights = string.Format("\"{0}\"", string.Join("\", \"", GetInstalledGamesName()));
                string allrightsReserved = string.Format("{0} All Rights Reserved.", Properties.Resources.GamePublisher);
                textBox_Copyrights.Text += string.Format("{0} are {1}.\r\n", installedGameRights, allrightsReserved);
            }
            textBox_Copyrights.Text += Properties.Resources.Copyrights;
            linkLabel_Support.Text = Properties.Resources.SupportLabelText;
            linkLabel_License.Text = Properties.Strings.ShowLicense;

            button_Ok.Focus();
        }

        private string[] GetInstalledGamesName()
        {
            List<AppInfo> appInfo = AppInfo.GetList();
            string[] gameList = new string[appInfo.Count];
            int i = 0;
            AppInfo.GetList().ForEach(info =>
            {
                gameList[i++] = info.Name;
            });
            return gameList;
        }

        private void LinkLabel_Support_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.SupportLabelURL);
        }

        private void Button_Ok_Click(object sender, EventArgs e) => Close();

        private void LinkLabel_License_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            (new LicensesForm()
            {
                ShowInTaskbar = false,
            }).ShowDialog(this);
        }

        private void LinkLabel_Github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.GithubURL);
        }
    }
}
