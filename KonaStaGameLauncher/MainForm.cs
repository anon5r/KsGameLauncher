using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Text.Json;

#if DEBUG
using System.Diagnostics;
#endif

namespace KonaStaGameLauncher
{
    public partial class MainForm : Form
    {

        private NotifyIcon notifyIcon;

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
            get {
                const int CP_NOCLOSE_BUTTON = 0x200;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_NOCLOSE_BUTTON;
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

        private void CreateNotificationIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.app;
            notifyIcon.Text = Resources.AppName;
            notifyIcon.Visible = true;

            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            menuStripMain = InitializeGameList(GetJson());
            
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

        private string GetJson()
        {
            string json;
            if (File.Exists(Properties.Settings.Default.appInfoLocal))
            {
                // Load appinfo.json from local
                using (StreamReader jsonStream = File.OpenText(Path.GetFullPath(Properties.Settings.Default.appInfoLocal)))
                {
                    json = jsonStream.ReadToEnd();
                }
#if DEBUG
                Debug.Write(json);
#endif
            }
            else
            {
                // Load appinfo.json from the internet
                // TODO Download from `Properties.Settings.Default.appInfoURL`
                using (StreamReader jsonStream = File.OpenText(Path.GetFullPath(Properties.Settings.Default.appInfoURL)))
                {
                    json = jsonStream.ReadToEnd();
                }
#if DEBUG
                Debug.WriteLine(json);
#endif
            }
            return json;
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
                            MessageBox.Show(appInfo.Name + " => " + appInfo.Login.URL + "\n" + appInfo.Login.Xpath);
                            System.Diagnostics.Process.Start(appInfo.Login.URL);
                            // TODO Launcher.startGame(appInfo)
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
                menu.Items.Clear();
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Resources.NoInstalledGames;
                item.Enabled = false;
                item.AutoSize = true;
                menu.Items.Add(item);
            }


            /// TODO インストール済みゲームリストからとゲーム情報を取得
            /// TODO ゲーム情報: { ゲーム名, ゲーム起動パス, ゲーム起動URL, ゲームアイコン }
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
                Application.Exit();
            }
                
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowInTaskbar = false;
            aboutForm.ShowDialog();

        }

        private void ManageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountForm accountForm = new AccountForm();
            accountForm.Show();
        }
    }
}
