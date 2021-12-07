using System;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            checkBox_UseProxy.Checked = Properties.Settings.Default.UseProxy;
            Text = Resources.OptionsWindowTitle;
            checkBox_UseProxy.Text = Resources.OptionsUseProxyText;
            linkLabel_OpenProxySettings.Text = Resources.OptionsProxySettingsLink;
            button_Save.Text = Resources.ButtonSave;
        }

        private void LinkLabel_OpenProxySettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.Common.OpenControlPanel("inetcpl.cpl,,4");
        }

        private void CheckBox_UseProxy_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            // Save using proxy
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
