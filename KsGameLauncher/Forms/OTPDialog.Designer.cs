namespace KsGameLauncher.Forms
{
    partial class OTPDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_Description = new System.Windows.Forms.Label();
            this.button_Ok = new System.Windows.Forms.Button();
            this.maskedTextBox_OTPCode = new System.Windows.Forms.MaskedTextBox();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_Description
            // 
            this.label_Description.AutoSize = true;
            this.label_Description.Location = new System.Drawing.Point(10, 19);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(199, 12);
            this.label_Description.TabIndex = 1;
            this.label_Description.Text = "Enter 6-digits code you received here";
            // 
            // button_Ok
            // 
            this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Ok.Location = new System.Drawing.Point(66, 81);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(75, 23);
            this.button_Ok.TabIndex = 1;
            this.button_Ok.Text = "OK";
            this.button_Ok.UseVisualStyleBackColor = true;
            this.button_Ok.Click += new System.EventHandler(this.Button_Ok_Click);
            // 
            // maskedTextBox_OTPCode
            // 
            this.maskedTextBox_OTPCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maskedTextBox_OTPCode.AsciiOnly = true;
            this.maskedTextBox_OTPCode.Culture = new System.Globalization.CultureInfo("");
            this.maskedTextBox_OTPCode.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.maskedTextBox_OTPCode.Location = new System.Drawing.Point(12, 56);
            this.maskedTextBox_OTPCode.Mask = "000000";
            this.maskedTextBox_OTPCode.Name = "maskedTextBox_OTPCode";
            this.maskedTextBox_OTPCode.ResetOnSpace = false;
            this.maskedTextBox_OTPCode.Size = new System.Drawing.Size(285, 19);
            this.maskedTextBox_OTPCode.TabIndex = 0;
            this.maskedTextBox_OTPCode.TextChanged += new System.EventHandler(this.MaskedTextBox_OTPCode_TextChanged);
            this.maskedTextBox_OTPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MaskedTextBox_OTPCode_KeyDown);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(167, 81);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.Button_Cancel_Click);
            // 
            // OTPDialog
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(309, 111);
            this.Controls.Add(this.maskedTextBox_OTPCode);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Ok);
            this.Controls.Add(this.label_Description);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OTPDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter the OTP";
            this.Load += new System.EventHandler(this.OTPDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.MaskedTextBox maskedTextBox_OTPCode;
        private System.Windows.Forms.Button button_Cancel;
    }
}