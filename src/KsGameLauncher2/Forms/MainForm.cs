namespace KsGameLauncher2.Forms
{
    public partial class MainForm : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // ユーザーが閉じる操作をした場合、キャンセル
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // フォームを閉じるのをキャンセル
            }
            base.OnFormClosing(e);
        }


        public MainForm()
        {
            InitializeComponent();
        }
    }
}
