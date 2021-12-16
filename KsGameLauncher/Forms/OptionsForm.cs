using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            string[] items = {
                // Normal
                Resources.ContextMenuSize_Text_Normal,
                // Large
                Resources.ContextMenuSize_Text_Large
            };
            comboBox_ContextMenuSize.Items.Clear();
            comboBox_ContextMenuSize.Items.AddRange(items);

            // Default values
            checkBox_UseProxy.Checked = Properties.Settings.Default.UseProxy;
            checkBox_Notification.Checked = Properties.Settings.Default.EnableNotification;
            checkBox_ConfirmExit.Checked = Properties.Settings.Default.ShowConfirmExit;
            comboBox_ContextMenuSize.SelectedIndex = Properties.Settings.Default.ContextMenuSize;

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
            bool menuSizeChanged =
                (Properties.Settings.Default.ContextMenuSize != comboBox_ContextMenuSize.SelectedIndex);
            // Save settings
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.EnableNotification = checkBox_Notification.Checked;
            Properties.Settings.Default.ShowConfirmExit = checkBox_ConfirmExit.Checked;
            Properties.Settings.Default.ContextMenuSize = comboBox_ContextMenuSize.SelectedIndex;
            Properties.Settings.Default.Save();
            
            if (menuSizeChanged)
                Program.mainForm.LoadGamesMenu();   // Re-load menu

            Close();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
