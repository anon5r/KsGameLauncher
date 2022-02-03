using System;
using System.Drawing;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class MainContext : ApplicationContext
    {
        internal MainForm mainForm;

        private NotifyIcon notifyIcon;

        internal static bool RunBackground = false;

        private bool _launcherMutex = false;

        /// <summary>
        /// Left clicked icon
        /// </summary>
        private NotifyIconContextMenuStrip menuStripMain;

        private Form _formWindow;

        public MainContext(MainForm _form) : base()
        {
            mainForm = _form;
            InitializeComponent();
            CreateNotificationIcon();
        }

        ~MainContext()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        private void OnQuit(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }


        private static NotifyIcon CreateNotifyIcon()
        {
            NotifyIcon notify = new NotifyIcon
            {
                Icon = Properties.Resources.appIcon,
                Text = Properties.Strings.AppName,
                Visible = true,
                BalloonTipTitle = Properties.Strings.AppName,
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

            if (RunBackground)
            {
                // Only show the exit menu when running in background mode
                menuStripMain = CreateMinimalMenuStripItems(true);
                contextMenuStrip_Sub = CreateMinimalMenuStripItems(true);
            }
            else
            {
                menuStripMain = InitGameMenu();
                contextMenuStrip_Sub = CreateMinimalMenuStripItems();
            }
        }

        /// <summary>
        /// Clicked notifyIcon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        // Display sub menu
                        contextMenuStrip_Sub.ShowContext(mainForm, PointToClient(Cursor.Position), notifyIcon);
                    }
                    break;

                case MouseButtons.Left:
                    {
                        // Display main menu (games)
                        menuStripMain.ShowContext(mainForm, PointToClient(Cursor.Position), notifyIcon);
                    }
                    break;
            }
        }

        private Point PointToClient(Point position)
        {
            return mainForm.PointToClient(position);
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

            item.Click += async delegate
            {
                if (_launcherMutex)
                {
                    MessageBox.Show(Properties.Strings.StartngLauncher, Properties.Strings.AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                _launcherMutex = true;
                try
                {
                    Launcher launcher = Launcher.Create();
                    await launcher.StartApp(appInfo);
                }
                catch (LoginCancelException)
                {
                    // Canceled process while login
                    return;
                }
                catch (LoginException ex)
                {
#if !DEBUG
                    MessageBox.Show(ex.Message, Properties.Strings.LoginExceptionDialogName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#else
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , Properties.Strings.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#endif
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
#if !DEBUG
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#endif
                    throw ex;
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
                Text = Properties.Strings.NoInstalledGames,
                Enabled = false,
                AutoSize = true,
            };
            NotifyIconContextMenuStrip menu = new NotifyIconContextMenuStrip();
            menu.Items.Add(item);

            return menu;
        }

        private NotifyIconContextMenuStrip CreateMinimalMenuStripItems()
        {
            return CreateMinimalMenuStripItems(false);
        }

        private NotifyIconContextMenuStrip CreateMinimalMenuStripItems(bool exitOnly)
        {
            NotifyIconContextMenuStrip menu = new NotifyIconContextMenuStrip();
            menu.Items.Clear();
            if (exitOnly)
            {
                // Run background
                ToolStripMenuItem item = new ToolStripMenuItem()
                {
                    Text = Properties.Strings.ContextMenuItems_Exit,
                    Enabled = true,
                };
                item.Click += ExitToolStripMenuItem_Click;
                menu.Items.Add(item);
            }
            else
            {
                // Normal
                aboutToolStripMenuItem.Text = Properties.Strings.ContextMenuItems_About;
                optionsToolStripMenuItem.Text = Properties.Strings.ContextMenuItems_Options;
                manageAccountsToolStripMenuItem.Text = Properties.Strings.ContextMenuItems_ManageAccount;
                addNewGameToolStripMenuItem.Text = Properties.Strings.ContextMenuItems_AddNewGame;
                exitToolStripMenuItem.Text = Properties.Strings.ContextMenuItems_Exit;
                
                menu = contextMenuStrip_Sub;
            }

            return menu;
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.ShowConfirmExit)
            {
                ExitingProcess();
                return;
            }

            DialogResult result = MessageBox.Show(Properties.Strings.ConfirmExitDialogMessage, Properties.Strings.AppName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.OK)
            {
                ExitingProcess();
            }
        }

        internal void ExitingProcess()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        internal void RestartProcess()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Restart();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formWindow == null || _formWindow.IsDisposed)
            {
                _formWindow = new AboutForm
                {
                    ShowInTaskbar = false,
                };
                _formWindow.FormClosed += delegate
                {
                    _formWindow?.Dispose();
                    _formWindow = null;
                };
                _formWindow.ShowDialog(mainForm);
            }
            else
                _formWindow.Activate();

        }

        private void ManageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formWindow == null || _formWindow.IsDisposed)
            {
                _formWindow = new AccountForm
                {
                    ShowInTaskbar = false
                };
                _formWindow.FormClosed += delegate
                {
                    _formWindow?.Dispose();
                    _formWindow = null;
                };
                _formWindow.ShowDialog(mainForm);
            }
            else
                _formWindow.Activate();

        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formWindow == null || _formWindow.IsDisposed)
            {
                _formWindow = new OptionsForm
                {
                    ShowInTaskbar = true
                };
                _formWindow.FormClosed += delegate
                {
                    _formWindow?.Dispose();
                    _formWindow = null;
                };
                _formWindow.ShowDialog(mainForm);
            }
            else
                _formWindow.Activate();
        }

        public void DisplayToolTip(string message, int timeout)
        {
            if (Properties.Settings.Default.EnableNotification)
            {
                if (notifyIcon == null)
                    notifyIcon = CreateNotifyIcon();

                notifyIcon.BalloonTipText = message;
                notifyIcon.ShowBalloonTip(timeout);
            }
        }

        private void AddNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formWindow == null || _formWindow.IsDisposed)
            {
                _formWindow = new AddNewGameForm()
                {
                    ShowInTaskbar = true,
                    ShowIcon = true,
                    Icon = Properties.Resources.appIcon,
                    AllowDrop = true,
                };
                _formWindow.FormClosed += delegate
                {
                    _formWindow?.Dispose();
                    _formWindow = null;
                };
                _formWindow.ShowDialog(mainForm);
            }
            else
                _formWindow.Activate();
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
