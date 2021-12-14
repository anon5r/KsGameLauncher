namespace KsGameLauncher
{
    partial class AccountForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountForm));
            this.button_Update = new System.Windows.Forms.Button();
            this.button_Remove = new System.Windows.Forms.Button();
            this.groupBox_AccountInfo = new System.Windows.Forms.GroupBox();
            this.label_UserAccountID_Value = new System.Windows.Forms.Label();
            this.label_AccountID = new System.Windows.Forms.Label();
            this.button_Close = new System.Windows.Forms.Button();
            this.groupBox_AccountInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Update
            // 
            this.button_Update.Location = new System.Drawing.Point(398, 22);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new System.Drawing.Size(98, 39);
            this.button_Update.TabIndex = 3;
            this.button_Update.Text = "&Update";
            this.button_Update.UseVisualStyleBackColor = true;
            this.button_Update.Click += new System.EventHandler(this.Button_Update_Click);
            // 
            // button_Remove
            // 
            this.button_Remove.Location = new System.Drawing.Point(398, 80);
            this.button_Remove.Name = "button_Remove";
            this.button_Remove.Size = new System.Drawing.Size(98, 39);
            this.button_Remove.TabIndex = 4;
            this.button_Remove.Text = "&Remove";
            this.button_Remove.UseVisualStyleBackColor = true;
            this.button_Remove.Click += new System.EventHandler(this.Button_Remove_Click);
            // 
            // groupBox_AccountInfo
            // 
            this.groupBox_AccountInfo.Controls.Add(this.label_UserAccountID_Value);
            this.groupBox_AccountInfo.Controls.Add(this.label_AccountID);
            this.groupBox_AccountInfo.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox_AccountInfo.Location = new System.Drawing.Point(12, 13);
            this.groupBox_AccountInfo.Name = "groupBox_AccountInfo";
            this.groupBox_AccountInfo.Size = new System.Drawing.Size(380, 160);
            this.groupBox_AccountInfo.TabIndex = 5;
            this.groupBox_AccountInfo.TabStop = false;
            this.groupBox_AccountInfo.Text = "Registered account";
            // 
            // label_UserAccountID_Value
            // 
            this.label_UserAccountID_Value.AutoSize = true;
            this.label_UserAccountID_Value.Location = new System.Drawing.Point(36, 67);
            this.label_UserAccountID_Value.Name = "label_UserAccountID_Value";
            this.label_UserAccountID_Value.Size = new System.Drawing.Size(146, 18);
            this.label_UserAccountID_Value.TabIndex = 1;
            this.label_UserAccountID_Value.Text = "User Account ID";
            // 
            // label_AccountID
            // 
            this.label_AccountID.AutoSize = true;
            this.label_AccountID.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_AccountID.Location = new System.Drawing.Point(24, 30);
            this.label_AccountID.Name = "label_AccountID";
            this.label_AccountID.Size = new System.Drawing.Size(91, 18);
            this.label_AccountID.TabIndex = 0;
            this.label_AccountID.Text = "Account ID";
            // 
            // button_Close
            // 
            this.button_Close.Location = new System.Drawing.Point(398, 134);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(98, 39);
            this.button_Close.TabIndex = 6;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.Button_Close_Click);
            // 
            // AccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 192);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.groupBox_AccountInfo);
            this.Controls.Add(this.button_Remove);
            this.Controls.Add(this.button_Update);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AccountForm";
            this.Load += new System.EventHandler(this.AccountForm_Load);
            this.groupBox_AccountInfo.ResumeLayout(false);
            this.groupBox_AccountInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_Remove;
        private System.Windows.Forms.Button button_Update;
        private System.Windows.Forms.GroupBox groupBox_AccountInfo;
        private System.Windows.Forms.Label label_UserAccountID_Value;
        private System.Windows.Forms.Label label_AccountID;
        private System.Windows.Forms.Button button_Close;
    }
}