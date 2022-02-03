﻿using KsGameLauncher.Utils;
using System;
using System.Diagnostics;
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
                Properties.Strings.ContextMenuSize_Text_Normal,
                // Large
                Properties.Strings.ContextMenuSize_Text_Large
            };
            comboBox_ContextMenuSize.Items.Clear();
            comboBox_ContextMenuSize.Items.AddRange(items);
            // Language
            Language currentLang = (string.IsNullOrEmpty(Properties.Settings.Default.Language) 
                || Properties.Settings.Default.Language == Properties.Resources.DefaultLanguage)
                ? Languages.GetLanguage(Properties.Resources.DefaultLanguage)
                : Languages.GetLanguage(Properties.Settings.Default.Language);
#if DEBUG
            Debug.WriteLine(String.Format("[OPTION:Load] Current language configuration: {0}", currentLang));
#endif
            foreach (Language lang in Languages.GetAvailableLanguages())
                comboBox_Languages.Items.Add(lang);
            comboBox_Languages.SelectedIndex = comboBox_Languages.Items.IndexOf(currentLang);
#if DEBUG
            Debug.WriteLine(String.Format("[OPTION:Load] Language comboBox selectedIndex: {0}", comboBox_Languages.Items.IndexOf(currentLang)));
#endif

            // Default values
            checkBox_UseProxy.Checked = Properties.Settings.Default.UseProxy;
            checkBox_Notification.Checked = Properties.Settings.Default.EnableNotification;
            checkBox_ConfirmExit.Checked = Properties.Settings.Default.ShowConfirmExit;
            checkBox_DisplayInstalledGamesOnly.Checked = Properties.Settings.Default.ShowOnlyInstalledGames;
            checkBox_RegisterCustomURI.Checked = Properties.Settings.Default.RegisterCustomURI;
            comboBox_ContextMenuSize.SelectedIndex = Properties.Settings.Default.ContextMenuSize;

            // String
            Text = Properties.Strings.OptionsWindowTitle;
            label_Language.Text = Properties.Strings.DisplayLanguage;
            label_ContextMenuSize.Text = Properties.Strings.LabelContextMenuSizeText;
            checkBox_UseProxy.Text = Properties.Strings.OptionsUseProxyText;
            checkBox_Notification.Text = Properties.Strings.OptionsDisplayNotification;
            checkBox_ConfirmExit.Text = Properties.Strings.ShowConfirmExit;
            checkBox_DisplayInstalledGamesOnly.Text = Properties.Strings.ShowOnlyInstalledGames;
            linkLabel_OpenProxySettings.Text = Properties.Strings.OptionsProxySettingsLink;
            button_Save.Text = Properties.Strings.ButtonSave;
            button_SyncAppInfo.Text = Properties.Strings.SynchWithServerButton;

            checkBox_RegisterCustomURI.Text = (checkBox_RegisterCustomURI.Checked)
                ? Properties.Strings.ShortcutLaunchCheckboxDisable
                : Properties.Strings.ShortcutLaunchCheckboxEnable;
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

            bool restartApp = false;

            // Language settings
            Language selectedLang = (Language)comboBox_Languages.Items[comboBox_Languages.SelectedIndex];
#if DEBUG
            Debug.WriteLine(String.Format("[OPTION:Save] Language before changes: {0}", Properties.Settings.Default.Language));
            Debug.WriteLine(String.Format("[OPTION:Save] Language selected: {0}", selectedLang.ID));
#endif
            if (selectedLang.ID != Properties.Settings.Default.Language)
            {
                // Confirming to restart app if the language configuration has been changed
                DialogResult confirm = MessageBox.Show(Properties.Strings.ConfirmApplicationRestartingForLanguageSettings,
                    Properties.Strings.ApplicationRequiresRestarting,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (confirm == DialogResult.Cancel)
                    return;
                if (confirm == DialogResult.Yes)
                    restartApp = true;
            }

            // Save settings
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.EnableNotification = checkBox_Notification.Checked;
            Properties.Settings.Default.ShowConfirmExit = checkBox_ConfirmExit.Checked;
            Properties.Settings.Default.ShowOnlyInstalledGames = checkBox_DisplayInstalledGamesOnly.Checked;
            Properties.Settings.Default.ContextMenuSize = comboBox_ContextMenuSize.SelectedIndex;
            Properties.Settings.Default.Language = ((Language)comboBox_Languages.Items[comboBox_Languages.SelectedIndex]).ID;
            Properties.Settings.Default.Save();

            if (needsUpdateGames)
                Program.mainContext.LoadGamesMenu();   // Re-load menu

            if (restartApp)
            {
                Program.mainContext.RestartProcess();
            }

            Close();
        }

        private void OptionsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private async void Button_SyncAppInfo_Click(object sender, EventArgs e)
        {

            var dialogResult = MessageBox.Show(Properties.Strings.SyncWithServerConfirmMessage, Properties.Strings.SyncWithServerDialogTitle,
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

                        MessageBox.Show(Properties.Strings.SyncWithServerSuccessMessage, Properties.Strings.SyncWithServerDialogTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Strings.SyncWithServerFailedMessage, Properties.Strings.SyncWithServerDialogTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    MessageBox.Show(Properties.Strings.ErrorGetAppInfoFailed, Properties.Strings.SyncWithServerDialogTitle,
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
                string message = string.Format(Properties.Strings.ConfirmRegisterCustomURI, Properties.Settings.Default.AppUriScheme);
                dialogResult = MessageBox.Show(message, Properties.Strings.AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    // Enable shortcut launch mode / Register custom URI scheme
                    AppUtil.RegisterScheme();
                    checkBox_RegisterCustomURI.Text = Properties.Strings.ShortcutLaunchCheckboxDisable;
                    Properties.Settings.Default.RegisterCustomURI = (state == CheckState.Checked);
                    Properties.Settings.Default.Save();
                    MessageBox.Show(Properties.Strings.Enabled, Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    checkBox_RegisterCustomURI.Checked = false;
            }
            else
            {
                dialogResult = MessageBox.Show(Properties.Strings.ConfirmUnregisterCustomURI, Properties.Strings.AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    // Disable shortcut launch mode / Remove custom URI scheme
                    AppUtil.DeleteScheme();
                    checkBox_RegisterCustomURI.Text = Properties.Strings.ShortcutLaunchCheckboxEnable;
                    Properties.Settings.Default.RegisterCustomURI = (state == CheckState.Checked);
                    Properties.Settings.Default.Save();
                    MessageBox.Show(Properties.Strings.Disabled, Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    checkBox_RegisterCustomURI.Checked = true;
            }
        }
    }
}
