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
            this.checkBox_UseProxy = new System.Windows.Forms.CheckBox();
            this.linkLabel_OpenProxySettings = new System.Windows.Forms.LinkLabel();
            this.button_Save = new System.Windows.Forms.Button();
            this.checkBox_Notification = new System.Windows.Forms.CheckBox();
            this.checkBox_ConfirmExit = new System.Windows.Forms.CheckBox();
            this.comboBox_ContextMenuSize = new System.Windows.Forms.ComboBox();
            this.label_ContextMenuSize = new System.Windows.Forms.Label();
            this.button_SyncAppInfo = new System.Windows.Forms.Button();
            this.checkBox_DisplayInstalledGamesOnly = new System.Windows.Forms.CheckBox();
            this.checkBox_RegisterCustomURI = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_UseProxy
            // 
            this.checkBox_UseProxy.AutoSize = true;
            this.checkBox_UseProxy.Location = new System.Drawing.Point(15, 27);
            this.checkBox_UseProxy.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_UseProxy.Name = "checkBox_UseProxy";
            this.checkBox_UseProxy.Size = new System.Drawing.Size(76, 16);
            this.checkBox_UseProxy.TabIndex = 0;
            this.checkBox_UseProxy.Text = "Use proxy";
            this.checkBox_UseProxy.UseVisualStyleBackColor = true;
            // 
            // linkLabel_OpenProxySettings
            // 
            this.linkLabel_OpenProxySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel_OpenProxySettings.AutoSize = true;
            this.linkLabel_OpenProxySettings.Location = new System.Drawing.Point(112, 43);
            this.linkLabel_OpenProxySettings.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel_OpenProxySettings.Name = "linkLabel_OpenProxySettings";
            this.linkLabel_OpenProxySettings.Size = new System.Drawing.Size(108, 12);
            this.linkLabel_OpenProxySettings.TabIndex = 1;
            this.linkLabel_OpenProxySettings.TabStop = true;
            this.linkLabel_OpenProxySettings.Text = "Open proxy settings";
            this.linkLabel_OpenProxySettings.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabel_OpenProxySettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_OpenProxySettings_LinkClicked);
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Location = new System.Drawing.Point(166, 264);
            this.button_Save.Margin = new System.Windows.Forms.Padding(2);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(54, 25);
            this.button_Save.TabIndex = 3;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // checkBox_Notification
            // 
            this.checkBox_Notification.AutoSize = true;
            this.checkBox_Notification.Checked = true;
            this.checkBox_Notification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Notification.Location = new System.Drawing.Point(12, 71);
            this.checkBox_Notification.Name = "checkBox_Notification";
            this.checkBox_Notification.Size = new System.Drawing.Size(123, 16);
            this.checkBox_Notification.TabIndex = 2;
            this.checkBox_Notification.Text = "Display notification";
            this.checkBox_Notification.UseVisualStyleBackColor = true;
            // 
            // checkBox_ConfirmExit
            // 
            this.checkBox_ConfirmExit.AutoSize = true;
            this.checkBox_ConfirmExit.Checked = true;
            this.checkBox_ConfirmExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ConfirmExit.Location = new System.Drawing.Point(12, 93);
            this.checkBox_ConfirmExit.Name = "checkBox_ConfirmExit";
            this.checkBox_ConfirmExit.Size = new System.Drawing.Size(116, 16);
            this.checkBox_ConfirmExit.TabIndex = 4;
            this.checkBox_ConfirmExit.Text = "Show confirm exit";
            this.checkBox_ConfirmExit.UseVisualStyleBackColor = true;
            // 
            // comboBox_ContextMenuSize
            // 
            this.comboBox_ContextMenuSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox_ContextMenuSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ContextMenuSize.FormattingEnabled = true;
            this.comboBox_ContextMenuSize.Location = new System.Drawing.Point(10, 174);
            this.comboBox_ContextMenuSize.Name = "comboBox_ContextMenuSize";
            this.comboBox_ContextMenuSize.Size = new System.Drawing.Size(121, 20);
            this.comboBox_ContextMenuSize.TabIndex = 5;
            // 
            // label_ContextMenuSize
            // 
            this.label_ContextMenuSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_ContextMenuSize.AutoSize = true;
            this.label_ContextMenuSize.Location = new System.Drawing.Point(10, 159);
            this.label_ContextMenuSize.Name = "label_ContextMenuSize";
            this.label_ContextMenuSize.Size = new System.Drawing.Size(100, 12);
            this.label_ContextMenuSize.TabIndex = 6;
            this.label_ContextMenuSize.Text = "Context menu size";
            // 
            // button_SyncAppInfo
            // 
            this.button_SyncAppInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_SyncAppInfo.Location = new System.Drawing.Point(10, 200);
            this.button_SyncAppInfo.Name = "button_SyncAppInfo";
            this.button_SyncAppInfo.Size = new System.Drawing.Size(100, 23);
            this.button_SyncAppInfo.TabIndex = 7;
            this.button_SyncAppInfo.TabStop = false;
            this.button_SyncAppInfo.Text = "Sync with server";
            this.button_SyncAppInfo.UseVisualStyleBackColor = true;
            this.button_SyncAppInfo.Click += new System.EventHandler(this.Button_SyncAppInfo_Click);
            // 
            // checkBox_DisplayInstalledGamesOnly
            // 
            this.checkBox_DisplayInstalledGamesOnly.AutoSize = true;
            this.checkBox_DisplayInstalledGamesOnly.Checked = true;
            this.checkBox_DisplayInstalledGamesOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_DisplayInstalledGamesOnly.Location = new System.Drawing.Point(12, 115);
            this.checkBox_DisplayInstalledGamesOnly.Name = "checkBox_DisplayInstalledGamesOnly";
            this.checkBox_DisplayInstalledGamesOnly.Size = new System.Drawing.Size(171, 16);
            this.checkBox_DisplayInstalledGamesOnly.TabIndex = 4;
            this.checkBox_DisplayInstalledGamesOnly.Text = "Display installed games only";
            this.checkBox_DisplayInstalledGamesOnly.UseVisualStyleBackColor = true;
            // 
            // checkBox_RegisterCustomURI
            // 
            this.checkBox_RegisterCustomURI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_RegisterCustomURI.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_RegisterCustomURI.AutoSize = true;
            this.checkBox_RegisterCustomURI.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox_RegisterCustomURI.Location = new System.Drawing.Point(10, 229);
            this.checkBox_RegisterCustomURI.Name = "checkBox_RegisterCustomURI";
            this.checkBox_RegisterCustomURI.Size = new System.Drawing.Size(124, 22);
            this.checkBox_RegisterCustomURI.TabIndex = 8;
            this.checkBox_RegisterCustomURI.Text = "Register Custom URI";
            this.checkBox_RegisterCustomURI.UseVisualStyleBackColor = true;
            this.checkBox_RegisterCustomURI.Click += new System.EventHandler(this.CheckBox_RegisterCustomURI_Click);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.button_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(227, 297);
            this.Controls.Add(this.checkBox_RegisterCustomURI);
            this.Controls.Add(this.button_SyncAppInfo);
            this.Controls.Add(this.label_ContextMenuSize);
            this.Controls.Add(this.comboBox_ContextMenuSize);
            this.Controls.Add(this.checkBox_DisplayInstalledGamesOnly);
            this.Controls.Add(this.checkBox_ConfirmExit);
            this.Controls.Add(this.checkBox_Notification);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.linkLabel_OpenProxySettings);
            this.Controls.Add(this.checkBox_UseProxy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OptionsForm_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_UseProxy;
        private System.Windows.Forms.LinkLabel linkLabel_OpenProxySettings;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.CheckBox checkBox_Notification;
        private System.Windows.Forms.CheckBox checkBox_ConfirmExit;
        private System.Windows.Forms.ComboBox comboBox_ContextMenuSize;
        private System.Windows.Forms.Label label_ContextMenuSize;
        private System.Windows.Forms.Button button_SyncAppInfo;
        private System.Windows.Forms.CheckBox checkBox_DisplayInstalledGamesOnly;
        private System.Windows.Forms.CheckBox checkBox_RegisterCustomURI;
    }
}