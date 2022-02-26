using KsGameLauncher.Utils;
using KsGameLauncher.Structures;
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
            string[] menuSizes = {
                // Normal
                Properties.Strings.ContextMenuSize_Text_Normal,
                // Large
                Properties.Strings.ContextMenuSize_Text_Large
            };
            comboBox_ContextMenuSize.Items.Clear();
            comboBox_ContextMenuSize.Items.AddRange(menuSizes);
            // Languages
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

            // Check update
            int[] updateIntervalsDays = { 1, 3, 7, 14 }; // None, everyday, 3 days, 7 days, 2 weeks
            comboBox_CheckInterval.Items.Clear();
            comboBox_CheckInterval.Items.Add(new CheckInterval(0, CheckInterval.UnitType.None));
            for (int i = 0; i < updateIntervalsDays.Length; i++)
            {
                CheckInterval check = new CheckInterval(updateIntervalsDays[i], CheckInterval.UnitType.Days);
                comboBox_CheckInterval.Items.Add(check);
            }
#if DEBUG
            for (int i = 0; i < updateIntervalsDays.Length; i++)
            {
                CheckInterval check = new CheckInterval(updateIntervalsDays[i], CheckInterval.UnitType.Minutes);
                comboBox_CheckInterval.Items.Add(check);
            }
#endif



            // Default values
            checkBox_UseProxy.Checked = Properties.Settings.Default.UseProxy;
            checkBox_Notification.Checked = Properties.Settings.Default.EnableNotification;
            checkBox_ConfirmExit.Checked = Properties.Settings.Default.ShowConfirmExit;
            checkBox_DisplayInstalledGamesOnly.Checked = Properties.Settings.Default.ShowOnlyInstalledGames;
            checkBox_RunDirect.Checked = Properties.Settings.Default.RunGameDirect;
            checkBox_RegisterCustomURI.Checked = Properties.Settings.Default.RegisterCustomURI;
            comboBox_ContextMenuSize.SelectedIndex = Properties.Settings.Default.ContextMenuSize;
            int intervalSelectedIndex = 0;
            if (Properties.Settings.Default.UpdateCheckInterval > 0)
            {
                int settingInterval = Properties.Settings.Default.UpdateCheckInterval;
                CheckInterval.UnitType settingUnit = Properties.Settings.Default.UpdateCheckIntervalUnit;
                for (int i = 0; i < comboBox_CheckInterval.Items.Count; i++)
                {
                    CheckInterval intervalItem = (CheckInterval)comboBox_CheckInterval.Items[i];
                    if (intervalItem.Interval == settingInterval && intervalItem.Unit == settingUnit)
                    {
                        intervalSelectedIndex = i;
                        break;
                    }
                }
            }
            comboBox_CheckInterval.SelectedIndex = intervalSelectedIndex;

            // String
            Text = Properties.Strings.OptionsWindowTitle;
            label_Language.Text = Properties.Strings.DisplayLanguage;
            label_ContextMenuSize.Text = Properties.Strings.LabelContextMenuSizeText;
            checkBox_UseProxy.Text = Properties.Strings.OptionsUseProxyText;
            checkBox_Notification.Text = Properties.Strings.OptionsDisplayNotification;
            checkBox_ConfirmExit.Text = Properties.Strings.ShowConfirmExit;
            checkBox_DisplayInstalledGamesOnly.Text = Properties.Strings.ShowOnlyInstalledGames;
            checkBox_RunDirect.Text = Properties.Strings.RunGameDirectly;
            linkLabel_OpenProxySettings.Text = Properties.Strings.OptionsProxySettingsLink;
            button_Save.Text = Properties.Strings.ButtonSave;
            button_SyncAppInfo.Text = Properties.Strings.SynchWithServerButton;
            label_CheckAutoUpdateInterval.Text = Properties.Strings.CheckAutoUpdate;
            button_ManualCheck.Text = Properties.Strings.ManualUpdateCheckButton;

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

            // AutoUpdate settings
            CheckInterval selectedInterval = (CheckInterval)comboBox_CheckInterval.Items[comboBox_CheckInterval.SelectedIndex];
#if DEBUG
            Debug.WriteLine(String.Format("[OPTION:Save] AutoUpdateCheck.Interval before changes: {0}", Properties.Settings.Default.UpdateCheckInterval));
            Debug.WriteLine(String.Format("[OPTION:Save] AutoUpdateCheck.Interval before unit: {0}", Properties.Settings.Default.UpdateCheckIntervalUnit));
            Debug.WriteLine(String.Format("[OPTION:Save] AutoUpdateCheck.Interval selected: {0}", selectedInterval.Interval));
            Debug.WriteLine(String.Format("[OPTION:Save] AutoUpdateCheck.Interval unit: {0}", selectedInterval.Unit));
#endif
            // need to update thread interval
            bool reloadAutoUpdate = false;
            if (Properties.Settings.Default.UpdateCheckInterval != selectedInterval.Interval
                || Properties.Settings.Default.UpdateCheckIntervalUnit != selectedInterval.Unit)
                reloadAutoUpdate = true;


            // Save settings
            Properties.Settings.Default.UseProxy = checkBox_UseProxy.Checked;
            Properties.Settings.Default.EnableNotification = checkBox_Notification.Checked;
            Properties.Settings.Default.ShowConfirmExit = checkBox_ConfirmExit.Checked;
            Properties.Settings.Default.ShowOnlyInstalledGames = checkBox_DisplayInstalledGamesOnly.Checked;
            Properties.Settings.Default.RunGameDirect = checkBox_RunDirect.Checked;
            Properties.Settings.Default.ContextMenuSize = comboBox_ContextMenuSize.SelectedIndex;
            Properties.Settings.Default.Language = selectedLang.ID;
            Properties.Settings.Default.UpdateCheckInterval = selectedInterval.Interval;
            Properties.Settings.Default.UpdateCheckIntervalUnit = selectedInterval.Unit;
            Properties.Settings.Default.Save();

            if (needsUpdateGames)
                Program.mainContext.LoadGamesMenu();   // Reload menu

            if (reloadAutoUpdate)
                UpdateChecker.UpdateInterval(selectedInterval.Interval, selectedInterval.Unit);


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

        private void Button_ManualCheck_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Properties.Strings.ConfirmToExecuteCheckManualUpdate, 
                Properties.Strings.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (result == DialogResult.Yes)
                AppUtil.CheckUpdate();
        }
    }
}
