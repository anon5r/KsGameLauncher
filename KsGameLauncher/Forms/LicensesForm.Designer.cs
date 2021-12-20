namespace KsGameLauncher
{
    partial class LicensesForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_LicenseText = new System.Windows.Forms.TextBox();
            this.button_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_LicenseText
            // 
            this.textBox_LicenseText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_LicenseText.Location = new System.Drawing.Point(12, 12);
            this.textBox_LicenseText.Multiline = true;
            this.textBox_LicenseText.Name = "textBox_LicenseText";
            this.textBox_LicenseText.ReadOnly = true;
            this.textBox_LicenseText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_LicenseText.Size = new System.Drawing.Size(355, 363);
            this.textBox_LicenseText.TabIndex = 0;
            this.textBox_LicenseText.Text = "License agreement text";
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Close.Location = new System.Drawing.Point(153, 381);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 1;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.Button_Close_Click);
            // 
            // LicensesForm
            // 
            this.AcceptButton = this.button_Close;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CancelButton = this.button_Close;
            this.ClientSize = new System.Drawing.Size(379, 414);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.textBox_LicenseText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicensesForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Licenses";
            this.Load += new System.EventHandler(this.Licenses_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_LicenseText;
        private System.Windows.Forms.Button button_Close;
    }
}
