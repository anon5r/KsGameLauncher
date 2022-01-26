using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KsGameLauncher.Forms
{
    public partial class OTPDialog : Form
    {
        private string _code;

        public string Code
        {
            get
            {
                return _code;
            }
        }

        public OTPDialog()
        {
            InitializeComponent();
        }

        private void OTPDialog_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.appIcon;
            Text = Resources.AppName;
            button_Ok.Enabled = false;
        }

        public DialogResult ShowDialog(string message)
        {
            System.Media.SystemSounds.Beep.Play();
            label_Description.Text = message;
            return ShowDialog();
        }

        public DialogResult ShowDialog(int digits)
        {
            System.Media.SystemSounds.Beep.Play();
            textBox_OTPCode.MaxLength = digits;
            return ShowDialog();
        }

        public DialogResult ShowDialog(string message, int digits)
        {
            System.Media.SystemSounds.Beep.Play();
            label_Description.Text = message;
            return ShowDialog(digits);
        }



        private void Button_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            _code = textBox_OTPCode.Text.Trim();

            Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void TextBox_OTPCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox_OTPCode.MaxLength == textBox_OTPCode.TextLength)
                    Button_Ok_Click(sender, e);
            }
            else if (e.KeyData == (Keys.Control | Keys.A))
            {
                textBox_OTPCode.SelectAll();
            }
            else if (e.KeyData == (Keys.Control | Keys.C))
            {
                textBox_OTPCode.Copy();
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                textBox_OTPCode.Paste();
            }
        }

        private void TextBox_OTPCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void TextBox_OTPCode_TextChanged(object sender, EventArgs e)
        {
            textBox_OTPCode.Text = AllowStringAsNumeric(textBox_OTPCode.Text);
            if (textBox_OTPCode.MaxLength > textBox_OTPCode.TextLength)
                button_Ok.Enabled = false;
            else
                button_Ok.Enabled = true;
        }

        private string AllowStringAsNumeric(string text)
        {
            if (text.Length > 0 && !Regex.IsMatch(text, @"^\d+$"))
            {
                text = Regex.Replace(text, @"[^\d]+$", "");
            }
            return text;

        }
    }
}
