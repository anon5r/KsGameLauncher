using System;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            // Default values
            checkBox_UseProxy.Checked = Properties.Settings.Default.UseProxy;
            checkBox_Notification.Checked = Properties.Settings.Default.EnableNotification;
            checkBox_ConfirmExit.Checked = Properties.Settings.Default.ShowConfirmExit;

            // String
            Text = Resources.OptionsWindowTitle;
            checkBox_UseProxy.Text = Resources.OptionsUseProxyText;
            checkBox_Notification.Text = Resources.OptionsDisplayNotification;
            checkBox_ConfirmExit.Text = Resources.ShowConfirmExit;
            linkLabel_OpenProxySettings.Text = Resources.OptionsProxySettingsLink;
            button_Save.Text = Resources.ButtonSave;
        }

        private void LinkLabel_OpenProxySettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.Common.OpenControlPanel("inetcpl.cpl,,4");
        }


        private void Button_Save_Click(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.EnableNotification = checkBox_Notification.Checked;
            Properties.Settings.Default.ShowConfirmExit = checkBox_ConfirmExit.Checked;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
