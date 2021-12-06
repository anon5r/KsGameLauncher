namespace KsGameLauncher
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.checkBox_UseProxy = new System.Windows.Forms.CheckBox();
            this.linkLabel_OpenProxySettings = new System.Windows.Forms.LinkLabel();
            this.button_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox_UseProxy
            // 
            this.checkBox_UseProxy.AutoSize = true;
            this.checkBox_UseProxy.Location = new System.Drawing.Point(25, 41);
            this.checkBox_UseProxy.Name = "checkBox_UseProxy";
            this.checkBox_UseProxy.Size = new System.Drawing.Size(109, 22);
            this.checkBox_UseProxy.TabIndex = 0;
            this.checkBox_UseProxy.Text = "Use proxy";
            this.checkBox_UseProxy.UseVisualStyleBackColor = true;
            this.checkBox_UseProxy.CheckedChanged += new System.EventHandler(this.CheckBox_UseProxy_CheckedChanged);
            // 
            // linkLabel_OpenProxySettings
            // 
            this.linkLabel_OpenProxySettings.AutoSize = true;
            this.linkLabel_OpenProxySettings.Location = new System.Drawing.Point(22, 83);
            this.linkLabel_OpenProxySettings.Name = "linkLabel_OpenProxySettings";
            this.linkLabel_OpenProxySettings.Size = new System.Drawing.Size(157, 18);
            this.linkLabel_OpenProxySettings.TabIndex = 1;
            this.linkLabel_OpenProxySettings.TabStop = true;
            this.linkLabel_OpenProxySettings.Text = "Open proxy settings";
            this.linkLabel_OpenProxySettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_OpenProxySettings_LinkClicked);
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Location = new System.Drawing.Point(276, 108);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(90, 37);
            this.button_Save.TabIndex = 2;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 157);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.linkLabel_OpenProxySettings);
            this.Controls.Add(this.checkBox_UseProxy);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_UseProxy;
        private System.Windows.Forms.LinkLabel linkLabel_OpenProxySettings;
        private System.Windows.Forms.Button button_Save;
    }
}