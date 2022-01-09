using KsGameLauncher.Utils;
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
            Icon = Properties.Resources.appIcon;
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
            checkBox_RegisterCustomURI.Checked = Properties.Settings.Default.RegisterCustomURI;
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

            checkBox_RegisterCustomURI.Text = (checkBox_RegisterCustomURI.Checked)
                ? Resources.ShortcutLaunchCheckboxDisable
                : Resources.ShortcutLaunchCheckboxEnable;
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
                Program.mainContext.LoadGamesMenu();   // Re-load menu

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
                        Program.mainContext.LoadGamesMenu();

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

        private void CheckBox_RegisterCustomURI_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult;
            CheckState state = ((CheckBox)sender).CheckState;
            if (state == CheckState.Checked)
            {
                string message = string.Format(Resources.ConfirmRegisterCustomURI, Properties.Settings.Default.AppUriScheme);
                dialogResult = MessageBox.Show(message, Resources.AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    // Enable shortcut launch mode / Register custom URI scheme
                    AppUtil.RegisterScheme();
                    checkBox_RegisterCustomURI.Text = Resources.ShortcutLaunchCheckboxDisable;
                    Properties.Settings.Default.RegisterCustomURI = (state == CheckState.Checked);
                    Properties.Settings.Default.Save();
                    MessageBox.Show(Resources.Enabled, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    checkBox_RegisterCustomURI.Checked = false;
            }
            else
            {
                dialogResult = MessageBox.Show(Resources.ConfirmUnregisterCustomURI, Resources.AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    // Disable shortcut launch mode / Remove custom URI scheme
                    AppUtil.DeleteScheme();
                    checkBox_RegisterCustomURI.Text = Resources.ShortcutLaunchCheckboxEnable;
                    Properties.Settings.Default.RegisterCustomURI = (state == CheckState.Checked);
                    Properties.Settings.Default.Save();
                    MessageBox.Show(Resources.Disabled, Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    checkBox_RegisterCustomURI.Checked = true;
            }
        }
    }
}
