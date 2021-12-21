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
            button_Update.Text = Resources.ButtonUpdate;
            button_Remove.Text = Resources.ButtonRemove;
            button_Close.Text = Resources.ButtonClose;
            groupBox_AccountInfo.Text = Resources.GroupBoxAccountInfo;
            label_AccountID.Text = Resources.LabelAccountID;
        }

        ~AccountForm()
        {
            credential = null;
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.app;
            Text = Resources.AccountManagerForm;
            button_Update.Text = Resources.ButtonUpdate_Register;
            button_Remove.Enabled = false;
            if (!RefreshRegisteredAccounts())
            {
                // There are no account registered
                // Display credential input prompt
                bool save = true;
                credential = CredentialManager.PromptForCredentials(CredentialName, ref save,
                    Resources.EnterYourAccountPasswordPrompt, Resources.AppName, "");
                if (credential != null)
                {
                    credential.Domain = Properties.Resources.AuthorizeDomain;
                    CredentialManager.SaveCredentials(CredentialName, credential);
                }
                RefreshRegisteredAccounts();
            }
        }

        async private void Button_Remove_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Resources.ConfirmToRemoveAccountFromList, Resources.ConfirmToRemove, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (CredentialManager.RemoveCredentials(CredentialName))
                    {
                        await Launcher.Logout();
                        RefreshRegisteredAccounts();
                        MessageBox.Show(Resources.AccountRemoveSucceeded);
                    }
                    else
                    {
                        result = MessageBox.Show(Resources.AccountRemoveFailed, Resources.AppName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (result == DialogResult.Retry)
                        {
                            Utils.Common.OpenControlPanel("keymgr.dll,KRShowKeyMgr");
                        }
                    }
                }
                catch (CredentialAPIException)
                {
                    result = MessageBox.Show(Resources.AccountRemoveFailed, Resources.AppName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                    {
                        Utils.Common.OpenControlPanel("keymgr.dll,KRShowKeyMgr");
                    }
                }
            }
        }

        private void Button_Update_Click(object sender, EventArgs e)
        {
            bool save = true;
            string defaultUserName = "";
            if (credential != null)
                defaultUserName = credential.UserName;

            credential = CredentialManager.PromptForCredentials(CredentialName, ref save, Resources.EnterYourAccountPasswordPrompt, Resources.AppName, defaultUserName);
            if (credential != null)
            {
                credential.Domain = Properties.Resources.AuthorizeDomain;
                CredentialManager.SaveCredentials(CredentialName, credential);
            }
            if (save)
            {
                RefreshRegisteredAccounts();
            }
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool RefreshRegisteredAccounts()
        {
            label_UserAccountID_Value.Text = Resources.NoAccountRegistered;
            button_Update.Text = Resources.ButtonUpdate_Register;
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
                button_Update.Text = Resources.ButtonUpdate;
            }
            return list != null && list.Count > 0;
        }

    }
}
