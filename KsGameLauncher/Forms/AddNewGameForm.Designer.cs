namespace KsGameLauncher
{
    partial class AddNewGameForm
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
            this.groupBox_DragHere = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // groupBox_DragHere
            // 
            this.groupBox_DragHere.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_DragHere.Location = new System.Drawing.Point(12, 12);
            this.groupBox_DragHere.Name = "groupBox_DragHere";
            this.groupBox_DragHere.Size = new System.Drawing.Size(360, 237);
            this.groupBox_DragHere.TabIndex = 0;
            this.groupBox_DragHere.TabStop = false;
            this.groupBox_DragHere.Text = "Drag here";
            this.groupBox_DragHere.DragDrop += new System.Windows.Forms.DragEventHandler(this.AddNewGame_DragDrop);
            this.groupBox_DragHere.DragEnter += new System.Windows.Forms.DragEventHandler(this.AddNewGame_DragEnter);
            // 
            // AddNewGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.groupBox_DragHere);
            this.MinimizeBox = false;
            this.Name = "AddNewGameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddNewGame";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddNewGameForm_FormClosed);
            this.Load += new System.EventHandler(this.AddNewGame_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AddNewGame_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AddNewGame_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_DragHere;
    }
}