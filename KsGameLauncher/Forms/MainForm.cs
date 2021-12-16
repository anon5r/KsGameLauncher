﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading.Tasks;

#if DEBUG
using System.Diagnostics;
#endif

namespace KsGameLauncher
{
    public partial class MainForm : Form
    {

        private static NotifyIcon notifyIcon;

        private bool _launcherMutex = false;

        /// <summary>
        /// Left clicked icon
        /// </summary>
        private ContextMenuStrip menuStripMain;

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



        async private void CreateNotificationIcon()
        {
            notifyIcon = CreateNotifyIcon();

            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            LoadGamesMenu();
        }

        async public void LoadGamesMenu()
        {
            string Json = await GetJson();
            if (Json == null)
                menuStripMain = CreateInitialMenuStripItems();
            else
                menuStripMain = InitializeGameList(Json);
        }

        /// <summary>
        /// Clicked notifyIcon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = false;
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        // Display sub menu
                        contextMenuStrip_Sub.Show(this, PointToClient(Cursor.Position));
                    }
                    break;

                case MouseButtons.Left:
                    {
                        // Display main menu (games)
                        menuStripMain.Show(this, PointToClient(Cursor.Position));
                    }
                    break;
            }
        }



        async private Task<string> GetJson()
        {
            string json;
            if (!File.Exists(Properties.Settings.Default.appInfoLocal))
            {
                // Load appinfo.json from the internet
                // Download from `Properties.Settings.Default.appInfoURL`
                await Utils.AppUtil.DownloadJson();
            }

            try
            {
                // Load appinfo.json from local
                using (StreamReader jsonStream = File.OpenText(Path.GetFullPath(Properties.Settings.Default.appInfoLocal)))
                {
                    json = jsonStream.ReadToEnd();
                }

#if DEBUG
                Debug.Write(json);
#endif

                return json;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(String.Format(Resources.FailedToLoadFile, Properties.Settings.Default.appInfoLocal),
                    Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private ContextMenuStrip InitializeGameList(string json)
        {
            ContextMenuStrip menu = new ContextMenuStrip
            {
                AutoClose = true,
                
            };
            menu.Items.Clear();

            Font font;
            int iconSize;
            switch (Properties.Settings.Default.ContextMenuSize)
            {
                case 1:
                    font = new Font(Control.DefaultFont.FontFamily, (float)(Control.DefaultFont.Size * 1.5));
                    iconSize = 64;
                    break;
                case 0:
                default:
                    font = Control.DefaultFont;
                    iconSize = 16;
                    break;
            }

            try
            {
                JsonSerializer.Deserialize<List<AppInfo>>(json).ForEach(appInfo =>
                {
#if DEBUG
                    Debug.WriteLine(JsonSerializer.Serialize(appInfo));
#endif
                    ToolStripMenuItem item = new ToolStripMenuItem
                    {
                        ImageScaling = ToolStripItemImageScaling.None
                    };


                    if (Utils.GameRegistry.IsInstalled(appInfo.Name))
                    {
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
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                                , Resources.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (LauncherException ex)
                            {
                                MessageBox.Show(String.Format(
                                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                                , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format(
                                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                                , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            finally
                            {
                                _launcherMutex = false;
                            }

                        };
                        menu.Items.Add(item);
                    }
                });
                menu.ImageScalingSize = new Size(iconSize, iconSize);
            }
            catch (Exception ex)
            {
#if DEBUG

                MessageBox.Show("Failed to parse JSON file correctly."
                    + "\n\n" + ex.GetType().ToString() + ":\n" + ex.Message
                    + "\n\n" + ex.InnerException.GetType().ToString() + ":\n" + ex.InnerException.Message
                    , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }

            if (menu.Items.Count < 1)
            {
                menu = CreateInitialMenuStripItems();
            }

            return menu;
        }

        private ContextMenuStrip CreateInitialMenuStripItems()
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Text = Resources.NoInstalledGames,
                Enabled = false,
                AutoSize = true,
            };
            ContextMenuStrip menu = new ContextMenuStrip();
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
            AboutForm aboutForm = new AboutForm
            {
                ShowInTaskbar = false
            };
            aboutForm.ShowDialog(this);

        }

        private void ManageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AccountForm()).Show(this);
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new OptionsForm()).ShowDialog(this);
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
    }
}
