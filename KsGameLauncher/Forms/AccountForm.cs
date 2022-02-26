using AdysTech.CredentialManager;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class AccountForm : Form
    {
        private readonly string CredentialName = Properties.Resources.CredentialTarget;

        NetworkCredential credential;

        public AccountForm()
        {
            InitializeComponent();
            button_Update.Text = Properties.Strings.ButtonUpdate;
            button_Remove.Text = Properties.Strings.ButtonRemove;
            button_Close.Text = Properties.Strings.ButtonClose;
            groupBox_AccountInfo.Text = Properties.Strings.GroupBoxAccountInfo;
            label_AccountID.Text = Properties.Strings.LabelAccountID;
            toolTip_Hint.SetToolTip(checkBox_UseOTP, Properties.Strings.ToolTipHintOTPDescription);
            linkLabel_OTP.Text = Properties.Strings.WhatsOTP;
            checkBox_UseOTP.Text = Properties.Strings.UseOTP;
        }

        ~AccountForm()
        {
            credential = null;
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.appIcon;
            Text = Properties.Strings.AccountManagerForm;
            button_Update.Text = Properties.Strings.ButtonUpdate_Register;
            button_Remove.Enabled = false;
            checkBox_UseOTP.Checked = Properties.Settings.Default.UseOTP;
            if (!RefreshRegisteredAccounts())
            {
                try
                {

                    // There are no account registered
                    // Display credential input prompt
                    bool save = true;
                    credential = CredentialManager.PromptForCredentials(CredentialName, ref save,
                        Properties.Strings.EnterYourAccountPasswordPrompt, Properties.Strings.AppName, "");
                    if (credential != null)
                    {
                        credential.Domain = Properties.Resources.AuthorizeDomain;
                        CredentialManager.SaveCredentials(CredentialName, credential);
                    }

                }
                catch (ArgumentNullException)
                {
                }
                RefreshRegisteredAccounts();
            }
        }

        private async void Button_Remove_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Properties.Strings.ConfirmToRemoveAccountFromList, Properties.Strings.ConfirmToRemove,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (CredentialManager.RemoveCredentials(CredentialName))
                    {
                        await Launcher.Logout();
                        credential = CredentialManager.GetCredentials(CredentialName);
                        RefreshRegisteredAccounts();
                        MessageBox.Show(Properties.Strings.AccountRemoveSucceeded);
                    }
                    else
                    {
                        result = MessageBox.Show(Properties.Strings.AccountRemoveFailed, Properties.Strings.AppName,
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (result == DialogResult.Retry)
                        {
                            Utils.Common.OpenControlPanel("keymgr.dll,KRShowKeyMgr");
                        }
                    }
                }
                catch (CredentialAPIException)
                {
                    result = MessageBox.Show(Properties.Strings.AccountRemoveFailed, Properties.Strings.AppName,
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (result == DialogResult.Retry)
                    {
                        Utils.Common.OpenControlPanel("keymgr.dll,KRShowKeyMgr");
                    }
                }
            }
        }

        private async void Button_Update_Click(object sender, EventArgs e)
        {
            bool save = true;
            string defaultUserName = "";
            if (credential != null)
                defaultUserName = credential.UserName;
            try
            {

                credential = CredentialManager.PromptForCredentials(CredentialName, ref save,
                    Properties.Strings.EnterYourAccountPasswordPrompt, Properties.Strings.AppName, defaultUserName);
                if (credential != null)
                {
                    credential.Domain = Properties.Resources.AuthorizeDomain;
                    CredentialManager.SaveCredentials(CredentialName, credential);
                }

                if (save)
                {
                    await Launcher.Logout();
                    RefreshRegisteredAccounts();
                }
            }
            catch (ArgumentNullException)
            {

            }
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool RefreshRegisteredAccounts()
        {
            label_UserAccountID_Value.Text = Properties.Strings.NoAccountRegistered;
            button_Update.Text = Properties.Strings.ButtonUpdate_Register;
            button_Remove.Enabled = false;
            List<NetworkCredential> list = CredentialManager.EnumerateCredentials(CredentialName);
            if (list != null && list.Count > 0)
            {
                list.ForEach(credential =>
                {
                    this.credential = credential;
                    label_UserAccountID_Value.Text = credential.UserName;
                });
                button_Remove.Enabled = true;
                button_Update.Text = Properties.Strings.ButtonUpdate;
            }
            return list != null && list.Count > 0;
        }

        private void LinkLabel_OTP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.Common.OpenUrlByDefaultBrowser(Properties.Settings.Default.LinkUrlOTP);
        }

        private void AccountForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.UseOTP = checkBox_UseOTP.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
