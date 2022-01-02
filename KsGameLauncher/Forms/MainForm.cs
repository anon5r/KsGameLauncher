using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;


namespace KsGameLauncher
{
    public partial class MainForm : Form
    {
        private static NotifyIcon notifyIcon;

        private bool _launcherMutex = false;

        /// <summary>
        /// Left clicked icon
        /// </summary>
        private NotifyIconContextMenuStrip menuStripMain;

        /// <summary>
        /// Disabling "Close" button on control box
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int CP_NOCLOSE_BUTTON = 0x200;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return cp;
            }
        }

        private AboutForm _aboutForm;
        private AccountForm _accountForm;
        private OptionsForm _optionsForm;
        private AddNewGameForm _addNewGameForm;


        /// <summary>
        /// Finalizer
        /// </summary>
        public MainForm()
        {
            InitializeComponent();


            // Hide from taskbar
            CreateNotificationIcon();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            contextMenuStrip_Sub.AutoClose = true;

        }


        ~MainForm()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        private static NotifyIcon CreateNotifyIcon()
        {
            NotifyIcon notify = new NotifyIcon
            {
                Icon = Properties.Resources.app,
                Text = Resources.AppName,
                Visible = true,
                BalloonTipTitle = Resources.AppName,
            };

            return notify;
        }



        private void CreateNotificationIcon()
        {
            notifyIcon = CreateNotifyIcon();

            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            LoadGamesMenu();
        }

        public async void LoadGamesMenu()
        {
            string json = await Launcher.GetJson();
            if (json == null)
                menuStripMain = CreateInitialMenuStripItems();
            else
            {
                AppInfo.LoadFromJson(json);
            }
            menuStripMain = InitGameMenu();
        }

        /// <summary>
        /// Clicked notifyIcon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = false;
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        // Display sub menu
                        contextMenuStrip_Sub.ShowContext(this, PointToClient(Cursor.Position), notifyIcon);
                    }
                    break;

                case MouseButtons.Left:
                    {
                        // Display main menu (games)
                        menuStripMain.ShowContext(this, PointToClient(Cursor.Position), notifyIcon);
                    }
                    break;
            }
        }





        private NotifyIconContextMenuStrip InitGameMenu()
        {
            NotifyIconContextMenuStrip menu = new NotifyIconContextMenuStrip
            {
                AutoClose = true,
            };
            menu.Items.Clear();

            Font font = GetFontSize();
            int iconSize = GetIconSize();

            if (AppInfo.GetList().Count > 0)
            {

                foreach (AppInfo appInfo in AppInfo.GetList())
                {
                    //#if DEBUG
                    //                    Debug.WriteLine(JsonSerializer.Serialize(appInfo));
                    //#endif
                    if (Utils.GameRegistry.IsInstalled(appInfo.Name))
                    {
                        var item = CreateNewMenuItem(appInfo, font, iconSize);
                        menu.Items.Add(item);
                    }
                    else if (!Properties.Settings.Default.ShowOnlyInstalledGames)
                    {
                        var item = CreateNewMenuItem(appInfo, font, iconSize);
                        item.Enabled = false;
                        menu.Items.Add(item);
                    }
                }
            }
            menu.ImageScalingSize = new Size(iconSize, iconSize);


            if (menu.Items.Count < 1)
            {
                menu = CreateInitialMenuStripItems();
            }

            return menu;
        }

        private int GetIconSize()
        {
            switch (Properties.Settings.Default.ContextMenuSize)
            {
                case 1:
                    return Properties.Settings.Default.MenuIconSize_Large;
            }
            return Properties.Settings.Default.MenuIconSize_Default;
        }

        private Font GetFontSize()
        {
            Font font = Control.DefaultFont;
            switch (Properties.Settings.Default.ContextMenuSize)
            {
                case 1:
                    return new Font(Control.DefaultFont.FontFamily, (float)(Control.DefaultFont.Size * 1.5));
            }
            return font;
        }

        internal ToolStripMenuItem CreateNewMenuItem(AppInfo appInfo)
        {
            return CreateNewMenuItem(appInfo, GetFontSize(), GetIconSize());
        }

        internal ToolStripMenuItem CreateNewMenuItem(AppInfo appInfo, Font font, int iconSize)
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                ImageScaling = ToolStripItemImageScaling.None
            };

            item.Text = appInfo.Name;
            item.Font = font;
            item.Size = new Size(iconSize, iconSize);

            Icon icon = appInfo.GetIcon();

            if (icon != null)
                item.Image = (new Icon(icon, iconSize, iconSize)).ToBitmap();

            item.Click += delegate
            {
                if (_launcherMutex)
                {
                    MessageBox.Show(Resources.StartngLauncher, Resources.AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                _launcherMutex = true;
                try
                {
                    Launcher launcher = Launcher.Create();
                    launcher.StartApp(appInfo);
                }
                catch (LoginException ex)
                {
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , Resources.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (LauncherException ex)
                {
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    _launcherMutex = false;
                }

            };
            return item;
        }

        private NotifyIconContextMenuStrip CreateInitialMenuStripItems()
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Text = Resources.NoInstalledGames,
                Enabled = false,
                AutoSize = true,
            };
            NotifyIconContextMenuStrip menu = new NotifyIconContextMenuStrip();
            menu.Items.Add(item);

            return menu;
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.ShowConfirmExit)
            {
                ExitingProcess();
                return;
            }

            DialogResult result = MessageBox.Show(Resources.ConfirmExitDialogMessage, Resources.AppName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.OK)
            {
                ExitingProcess();
            }
        }

        private void ExitingProcess()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Properties.Settings.Default.Save();
            Application.Exit();
        }


        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_aboutForm == null || _aboutForm.IsDisposed)
                _aboutForm = new AboutForm
                {
                    ShowInTaskbar = false
                };

            if (!_aboutForm.Visible)
                _aboutForm.ShowDialog(this);
            else
                _aboutForm.Activate();

        }

        private void ManageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_accountForm == null || _accountForm.IsDisposed)
                _accountForm = new AccountForm
                {
                    ShowInTaskbar = false
                };

            if (!_accountForm.Visible)
                _accountForm.ShowDialog(this);
            else
                _accountForm.Activate();

        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_optionsForm == null || _optionsForm.IsDisposed)
                _optionsForm = new OptionsForm
                {
                    ShowInTaskbar = false
                };

            if (!_optionsForm.Visible)
                _optionsForm.ShowDialog(this);
            else
                _optionsForm.Activate();
        }

        public static void DisplayToolTip(string message, int timeout)
        {
            if (Properties.Settings.Default.EnableNotification)
            {
                if (notifyIcon == null)
                    notifyIcon = CreateNotifyIcon();

                notifyIcon.BalloonTipText = message;
                notifyIcon.ShowBalloonTip(timeout);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.app;
            Text = Resources.AppName;
        }


        private void AddNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_addNewGameForm == null || _addNewGameForm.IsDisposed)
                _addNewGameForm = new AddNewGameForm()
                {
                    ShowInTaskbar = false
                };

            if (!_addNewGameForm.Visible)
                _addNewGameForm.ShowDialog(this);
            else
                _addNewGameForm.Activate();
        }

        internal NotifyIconContextMenuStrip GetMenuStrip()
        {
            return menuStripMain;
        }

        internal void SetMenuStrip(NotifyIconContextMenuStrip menu)
        {
            menuStripMain = menu;
        }
    }
}
