namespace KsGameLauncher
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label_Application = new System.Windows.Forms.Label();
            this.button_Ok = new System.Windows.Forms.Button();
            this.label_Version = new System.Windows.Forms.Label();
            this.AppDeveloper = new System.Windows.Forms.Label();
            this.linkLabel_Support = new System.Windows.Forms.LinkLabel();
            this.label_SpecialThanks = new System.Windows.Forms.Label();
            this.textBox_Copyrights = new System.Windows.Forms.TextBox();
            this.linkLabel_License = new System.Windows.Forms.LinkLabel();
            this.label_Develop = new System.Windows.Forms.Label();
            this.linkLabel_Github = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label_Application
            // 
            resources.ApplyResources(this.label_Application, "label_Application");
            this.label_Application.Name = "label_Application";
            // 
            // button_Ok
            // 
            resources.ApplyResources(this.button_Ok, "button_Ok");
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.UseVisualStyleBackColor = true;
            this.button_Ok.Click += new System.EventHandler(this.Button_Ok_Click);
            // 
            // label_Version
            // 
            resources.ApplyResources(this.label_Version, "label_Version");
            this.label_Version.Name = "label_Version";
            // 
            // AppDeveloper
            // 
            resources.ApplyResources(this.AppDeveloper, "AppDeveloper");
            this.AppDeveloper.Name = "AppDeveloper";
            // 
            // linkLabel_Support
            // 
            resources.ApplyResources(this.linkLabel_Support, "linkLabel_Support");
            this.linkLabel_Support.Name = "linkLabel_Support";
            this.linkLabel_Support.TabStop = true;
            this.linkLabel_Support.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Support_LinkClicked);
            // 
            // label_SpecialThanks
            // 
            resources.ApplyResources(this.label_SpecialThanks, "label_SpecialThanks");
            this.label_SpecialThanks.Name = "label_SpecialThanks";
            // 
            // textBox_Copyrights
            // 
            resources.ApplyResources(this.textBox_Copyrights, "textBox_Copyrights");
            this.textBox_Copyrights.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox_Copyrights.Name = "textBox_Copyrights";
            this.textBox_Copyrights.ReadOnly = true;
            this.textBox_Copyrights.ShortcutsEnabled = false;
            this.textBox_Copyrights.TabStop = false;
            // 
            // linkLabel_License
            // 
            resources.ApplyResources(this.linkLabel_License, "linkLabel_License");
            this.linkLabel_License.Name = "linkLabel_License";
            this.linkLabel_License.TabStop = true;
            this.linkLabel_License.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_License_LinkClicked);
            // 
            // label_Develop
            // 
            resources.ApplyResources(this.label_Develop, "label_Develop");
            this.label_Develop.Name = "label_Develop";
            // 
            // linkLabel_Github
            // 
            resources.ApplyResources(this.linkLabel_Github, "linkLabel_Github");
            this.linkLabel_Github.Name = "linkLabel_Github";
            this.linkLabel_Github.TabStop = true;
            this.linkLabel_Github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Github_LinkClicked);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.button_Ok;
            this.CancelButton = this.button_Ok;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label_Develop);
            this.Controls.Add(this.linkLabel_License);
            this.Controls.Add(this.textBox_Copyrights);
            this.Controls.Add(this.label_SpecialThanks);
            this.Controls.Add(this.linkLabel_Github);
            this.Controls.Add(this.linkLabel_Support);
            this.Controls.Add(this.AppDeveloper);
            this.Controls.Add(this.label_Version);
            this.Controls.Add(this.button_Ok);
            this.Controls.Add(this.label_Application);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Application;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.Label AppDeveloper;
        private System.Windows.Forms.LinkLabel linkLabel_Support;
        private System.Windows.Forms.Label label_SpecialThanks;
        private System.Windows.Forms.TextBox textBox_Copyrights;
        private System.Windows.Forms.LinkLabel linkLabel_License;
        private System.Windows.Forms.Label label_Develop;
        private System.Windows.Forms.LinkLabel linkLabel_Github;
    }
}
