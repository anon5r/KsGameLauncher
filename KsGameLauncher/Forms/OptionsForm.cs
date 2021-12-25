using System;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            var cancel = new System.Windows.Forms.Button()
            {
                Visible = false,
            };
            cancel.Click += delegate
            {
                Close();
            };
            CancelButton = cancel;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.app;
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
            checkBox_DisplayInstalledGamesOnly.Checked = Properties.Settings.Default.ShowOnlyInstalledGames;
            comboBox_ContextMenuSize.SelectedIndex = Properties.Settings.Default.ContextMenuSize;

            // String
            Text = Resources.OptionsWindowTitle;
            label_ContextMenuSize.Text = Resources.LabelContextMenuSizeText;
            checkBox_UseProxy.Text = Resources.OptionsUseProxyText;
            checkBox_Notification.Text = Resources.OptionsDisplayNotification;
            checkBox_ConfirmExit.Text = Resources.ShowConfirmExit;
            checkBox_DisplayInstalledGamesOnly.Text = Resources.ShowOnlyInstalledGames;
            linkLabel_OpenProxySettings.Text = Resources.OptionsProxySettingsLink;
            button_Save.Text = Resources.ButtonSave;
            button_SyncAppInfo.Text = Resources.SynchWithServerButton;
        }

        private void LinkLabel_OpenProxySettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.Common.OpenControlPanel("inetcpl.cpl,,4");
        }


        private void Button_Save_Click(object sender, EventArgs e)
        {
            bool needsUpdateGames =
                (Properties.Settings.Default.ShowOnlyInstalledGames != checkBox_DisplayInstalledGamesOnly.Checked)
                || (Properties.Settings.Default.ContextMenuSize != comboBox_ContextMenuSize.SelectedIndex);
            // Save settings
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.EnableNotification = checkBox_Notification.Checked;
            Properties.Settings.Default.ShowConfirmExit = checkBox_ConfirmExit.Checked;
            Properties.Settings.Default.ShowOnlyInstalledGames = checkBox_DisplayInstalledGamesOnly.Checked;
            Properties.Settings.Default.ContextMenuSize = comboBox_ContextMenuSize.SelectedIndex;
            Properties.Settings.Default.Save();

            if (needsUpdateGames)
                Program.mainForm.LoadGamesMenu();   // Re-load menu

            Close();
        }

        private void OptionsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private async void Button_SyncAppInfo_Click(object sender, EventArgs e)
        {

            var dialogResult = MessageBox.Show(Resources.SyncWithServerConfirmMessage, Resources.SyncWithServerDialogTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.DefaultDesktopOnly);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool result = await Utils.AppUtil.DownloadJson();
                    if (result)
                    {
                        Program.mainForm.LoadGamesMenu();

                        MessageBox.Show(Resources.SyncWithServerSuccessMessage, Resources.SyncWithServerDialogTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(Resources.SyncWithServerFailedMessage, Resources.SyncWithServerDialogTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    MessageBox.Show(Resources.ErrorGetAppInfoFailed, Resources.SyncWithServerDialogTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }
    }
}
