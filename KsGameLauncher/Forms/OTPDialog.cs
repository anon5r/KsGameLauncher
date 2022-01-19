using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KsGameLauncher.Forms
{
    public partial class OTPDialog : Form
    {
        private string _code;

        public string Code {
            get {
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
            label_Description.Text = message;
            return ShowDialog();
        }

        public DialogResult ShowDialog(int digits)
        {
            maskedTextBox_OTPCode.Mask = new string('0', digits);
            return ShowDialog();
        }

        public DialogResult ShowDialog(string message, int digits)
        {
            maskedTextBox_OTPCode.Mask = new string('0', digits);
            label_Description.Text = message;
            return ShowDialog();
        }

        public DialogResult ShowDialog(string message, string format)
        {
            maskedTextBox_OTPCode.Mask = format;
            label_Description.Text = message;
            return ShowDialog();
        }



        private void Button_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            _code = maskedTextBox_OTPCode.Text.Trim();

            Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void MaskedTextBox_OTPCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (maskedTextBox_OTPCode.MaskCompleted)
                    Button_Ok_Click(sender, e);
            }
        }

        private void MaskedTextBox_OTPCode_TextChanged(object sender, EventArgs e)
        {
            if (!maskedTextBox_OTPCode.MaskCompleted)
                button_Ok.Enabled = false;
            else
                button_Ok.Enabled = true;
        }
    }
}
