using System;
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

        private NotifyIcon notifyIcon;

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
            this.CreateNotificationIcon();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

        }


        ~MainForm()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        async private void CreateNotificationIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.app,
                Text = Resources.AppName,
                Visible = true
            };

            notifyIcon.MouseClick += NotifyIcon_MouseClick;

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
                        contextMenuStrip_Sub.Show(this, PointToClient(Cursor.Position));
                    }
                    break;

                case MouseButtons.Left:
                    {
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
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Clear();


            try
            {
                JsonSerializer.Deserialize<List<AppInfo>>(json).ForEach(appInfo =>
                {
#if DEBUG
                    Debug.WriteLine(JsonSerializer.Serialize(appInfo));
#endif
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    if (Utils.GameRegistry.IsInstalled(appInfo.Name))
                    {
                        item.Text = appInfo.Name;
                        Image icon = appInfo.GetIcon().ToBitmap();
                        if (icon != null)
                            item.Image = icon;
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
            DialogResult result = MessageBox.Show(Resources.ConfirmToExit, Resources.AppName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.OK)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                Properties.Settings.Default.Save();
                Application.Exit();
            }

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm
            {
                ShowInTaskbar = false
            };
            aboutForm.ShowDialog();

        }

        private void ManageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AccountForm()).Show();
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new OptionsForm()).Show();
        }
    }
}
