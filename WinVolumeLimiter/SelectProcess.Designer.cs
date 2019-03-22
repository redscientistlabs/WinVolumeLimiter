namespace WinVolumeLimiter
{
    
    partial class SelectProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProcess));
            this.lbProcesses = new System.Windows.Forms.ListBox();
            this.btnSendList = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbProcesses
            // 
            this.lbProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbProcesses.BackColor = System.Drawing.Color.Black;
            this.lbProcesses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbProcesses.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lbProcesses.ForeColor = System.Drawing.Color.White;
            this.lbProcesses.FormattingEnabled = true;
            this.lbProcesses.Location = new System.Drawing.Point(0, 0);
            this.lbProcesses.Name = "lbProcesses";
            this.lbProcesses.ScrollAlwaysVisible = true;
            this.lbProcesses.Size = new System.Drawing.Size(320, 273);
            this.lbProcesses.TabIndex = 0;
            // 
            // btnSendList
            // 
            this.btnSendList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSendList.BackColor = System.Drawing.Color.Black;
            this.btnSendList.FlatAppearance.BorderSize = 0;
            this.btnSendList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendList.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnSendList.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnSendList.Location = new System.Drawing.Point(115, 290);
            this.btnSendList.Name = "btnSendList";
            this.btnSendList.Size = new System.Drawing.Size(186, 23);
            this.btnSendList.TabIndex = 1;
            this.btnSendList.Text = "Select Process";
            this.btnSendList.UseVisualStyleBackColor = false;
            this.btnSendList.Click += new System.EventHandler(this.btnSendList_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.BackColor = System.Drawing.Color.Black;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnCancel.Location = new System.Drawing.Point(12, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SelectProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(319, 325);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSendList);
            this.Controls.Add(this.lbProcesses);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectProcess";
            this.Text = "Select Process";
            this.Load += new System.EventHandler(this.WGH_HookToProcess_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbProcesses;
        private System.Windows.Forms.Button btnSendList;
        private System.Windows.Forms.Button btnCancel;
	}
}