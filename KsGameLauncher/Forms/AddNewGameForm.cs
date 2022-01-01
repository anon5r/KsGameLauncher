using KsGameLauncher.Structures;
using System;
using System.IO;
using System.Windows.Forms;

namespace KsGameLauncher
{
    public partial class AddNewGameForm : Form
    {
        public AddNewGameForm()
        {
            InitializeComponent();
        }

        private void AddNewGame_DragDrop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            try
            {

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = files[i];
                    var finfo = new FileInfo(fileName);
                    StreamReader fstream = File.OpenText(fileName);
                    if (!finfo.Extension.ToLower().Equals(".url"))
                    {
                        fstream.Close();
                        fstream = null;
                        throw new FileFormatException(Resources.IncorrectFileFormat);
                    }

                    InternetShortcut shortcut = InternetShortcut.Parse(fstream);

                    if (!shortcut.URL.Host.Contains("eagate.573.jp"))
                    {
                        fstream.Close();
                        fstream = null;
                        throw new FileFormatException(Resources.NotSupportedShortcut);
                    }

                    string gameID;
                    // Only Infinitas has a different URL
                    if (shortcut.URL.ToString().Contains("game/infinitas/"))
                        gameID = "inifitas";
                    else
                        gameID = shortcut.URL.Query.Substring(
                            shortcut.URL.Query.LastIndexOf("game_id=") + "game_id=".Length);

                    string gameTitle = finfo.Name.Remove(finfo.Name.LastIndexOf("."));
                    // Create AppInfo object
                    AppInfo appInfo = new AppInfo()
                    {
                        Name = gameTitle,
                        ID = gameID,
                        IconPath = shortcut.IconFile,
                        Launch = new AppInfo.AppInfoLaunch()
                        {
                            Selector = Properties.Settings.Default.AppInfo_selector_default,
                            URL = shortcut.URL.ToString()
                        }
                    };

                    // Check existing
                    if (AppInfo.ContainID(appInfo.ID))
                    {
                        MessageBox.Show(string.Format(Resources.AlreadyGameExists, appInfo.Name), Resources.AppName,
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }

                    var result = MessageBox.Show(String.Format(Resources.ConfirmAddNewGame, appInfo.Name),
                        Resources.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                        MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
                    if (result == DialogResult.Yes)
                    {
                        // TODO AppInfoリストに追加し、JSON化して保存する
                        //JsonSerializer
                        AppInfo.GetList().Add(appInfo);
                        ToolStripMenuItem item = Program.mainForm.CreateNewMenuItem(appInfo);
                        NotifyIconContextMenuStrip menuStrip = Program.mainForm.GetMenuStrip();
                        menuStrip.Items.Add(item);
                        Program.mainForm.SetMenuStrip(menuStrip);
                    }
                }
            }
            catch (FileFormatException ex)
            {
                MessageBox.Show(ex.Message, Resources.AppName, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            // Save loaded games to local file
            AppInfo.Save();

        }

        private void AddNewGame_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void AddNewGame_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.app;
            Text = Resources.AddNewGameWindowTitle;
            groupBox_DragHere.Text = Resources.DropHere;
            Size = Properties.Settings.Default.NewGameFormSize;
        }

        private void AddNewGameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.NewGameFormSize = Size;
            Properties.Settings.Default.Save();
        }
    }
}
