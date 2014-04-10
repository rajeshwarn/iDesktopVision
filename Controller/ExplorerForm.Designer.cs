namespace Controller
{
    partial class ExplorerForm
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
            this.listRemoteFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textRemotePath = new System.Windows.Forms.TextBox();
            this.textLocalPath = new System.Windows.Forms.TextBox();
            this.listLocalFiles = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonReceive = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listRemoteFiles
            // 
            this.listRemoteFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listRemoteFiles.FullRowSelect = true;
            this.listRemoteFiles.GridLines = true;
            this.listRemoteFiles.Location = new System.Drawing.Point(514, 42);
            this.listRemoteFiles.Name = "listRemoteFiles";
            this.listRemoteFiles.Size = new System.Drawing.Size(434, 340);
            this.listRemoteFiles.TabIndex = 0;
            this.listRemoteFiles.UseCompatibleStateImageBehavior = false;
            this.listRemoteFiles.View = System.Windows.Forms.View.Details;
            this.listRemoteFiles.DoubleClick += new System.EventHandler(this.listViewItems_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File";
            this.columnHeader1.Width = 319;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 96;
            // 
            // textRemotePath
            // 
            this.textRemotePath.Location = new System.Drawing.Point(514, 12);
            this.textRemotePath.Name = "textRemotePath";
            this.textRemotePath.Size = new System.Drawing.Size(434, 20);
            this.textRemotePath.TabIndex = 1;
            this.textRemotePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPath_KeyDown);
            // 
            // textLocalPath
            // 
            this.textLocalPath.Location = new System.Drawing.Point(12, 12);
            this.textLocalPath.Name = "textLocalPath";
            this.textLocalPath.Size = new System.Drawing.Size(434, 20);
            this.textLocalPath.TabIndex = 3;
            this.textLocalPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textLocalPath_KeyDown);
            // 
            // listLocalFiles
            // 
            this.listLocalFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listLocalFiles.FullRowSelect = true;
            this.listLocalFiles.GridLines = true;
            this.listLocalFiles.Location = new System.Drawing.Point(12, 42);
            this.listLocalFiles.Name = "listLocalFiles";
            this.listLocalFiles.Size = new System.Drawing.Size(434, 340);
            this.listLocalFiles.TabIndex = 2;
            this.listLocalFiles.UseCompatibleStateImageBehavior = false;
            this.listLocalFiles.View = System.Windows.Forms.View.Details;
            this.listLocalFiles.DoubleClick += new System.EventHandler(this.listLocalFiles_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File";
            this.columnHeader3.Width = 319;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Size";
            this.columnHeader4.Width = 96;
            // 
            // buttonSend
            // 
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(463, 156);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(34, 23);
            this.buttonSend.TabIndex = 4;
            this.buttonSend.Text = ">>";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonReceive
            // 
            this.buttonReceive.Enabled = false;
            this.buttonReceive.Location = new System.Drawing.Point(463, 216);
            this.buttonReceive.Name = "buttonReceive";
            this.buttonReceive.Size = new System.Drawing.Size(34, 23);
            this.buttonReceive.TabIndex = 5;
            this.buttonReceive.Text = "<<";
            this.buttonReceive.UseVisualStyleBackColor = true;
            // 
            // ExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 394);
            this.Controls.Add(this.buttonReceive);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textLocalPath);
            this.Controls.Add(this.listLocalFiles);
            this.Controls.Add(this.textRemotePath);
            this.Controls.Add(this.listRemoteFiles);
            this.Name = "ExplorerForm";
            this.Text = "ExplorerForm";
            this.Load += new System.EventHandler(this.ExplorerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listRemoteFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox textRemotePath;
        private System.Windows.Forms.TextBox textLocalPath;
        private System.Windows.Forms.ListView listLocalFiles;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonReceive;
    }
}