namespace WinVolumeLimiter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnChooseProcess = new System.Windows.Forms.Button();
            this.updownSamplingDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tbProcessName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.updownRestoreDelay = new System.Windows.Forms.NumericUpDown();
            this.tbPercent = new System.Windows.Forms.TrackBar();
            this.audioLevelsControl = new WinVolumeLimiter.AudioLevelsUIControl();
            ((System.ComponentModel.ISupportInitialize)(this.updownSamplingDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownRestoreDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // btnChooseProcess
            // 
            this.btnChooseProcess.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnChooseProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChooseProcess.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnChooseProcess.Location = new System.Drawing.Point(12, 285);
            this.btnChooseProcess.Name = "btnChooseProcess";
            this.btnChooseProcess.Size = new System.Drawing.Size(106, 23);
            this.btnChooseProcess.TabIndex = 1;
            this.btnChooseProcess.Text = "Choose Process";
            this.btnChooseProcess.UseVisualStyleBackColor = false;
            this.btnChooseProcess.Click += new System.EventHandler(this.BtnChooseProcess_Click);
            // 
            // updownSamplingDelay
            // 
            this.updownSamplingDelay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.updownSamplingDelay.ForeColor = System.Drawing.Color.White;
            this.updownSamplingDelay.Location = new System.Drawing.Point(271, 305);
            this.updownSamplingDelay.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownSamplingDelay.Name = "updownSamplingDelay";
            this.updownSamplingDelay.Size = new System.Drawing.Size(72, 22);
            this.updownSamplingDelay.TabIndex = 2;
            this.updownSamplingDelay.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(241, 285);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sampling Delay (ms)";
            // 
            // tbProcessName
            // 
            this.tbProcessName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tbProcessName.Enabled = false;
            this.tbProcessName.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tbProcessName.ForeColor = System.Drawing.Color.White;
            this.tbProcessName.Location = new System.Drawing.Point(12, 310);
            this.tbProcessName.Name = "tbProcessName";
            this.tbProcessName.ReadOnly = true;
            this.tbProcessName.Size = new System.Drawing.Size(106, 22);
            this.tbProcessName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(132, 285);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Restore Delay (ms)";
            // 
            // updownRestoreDelay
            // 
            this.updownRestoreDelay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.updownRestoreDelay.ForeColor = System.Drawing.Color.White;
            this.updownRestoreDelay.Location = new System.Drawing.Point(156, 305);
            this.updownRestoreDelay.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.updownRestoreDelay.Name = "updownRestoreDelay";
            this.updownRestoreDelay.Size = new System.Drawing.Size(72, 22);
            this.updownRestoreDelay.TabIndex = 6;
            this.updownRestoreDelay.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // tbPercent
            // 
            this.tbPercent.LargeChange = 10;
            this.tbPercent.Location = new System.Drawing.Point(12, 337);
            this.tbPercent.Maximum = 130;
            this.tbPercent.Name = "tbPercent";
            this.tbPercent.Size = new System.Drawing.Size(331, 45);
            this.tbPercent.TabIndex = 8;
            this.tbPercent.TickFrequency = 10;
            this.tbPercent.Value = 100;
            // 
            // audioLevelsControl
            // 
            this.audioLevelsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.audioLevelsControl.AudioMonitor = null;
            this.audioLevelsControl.Location = new System.Drawing.Point(12, 12);
            this.audioLevelsControl.Margin = new System.Windows.Forms.Padding(0);
            this.audioLevelsControl.Name = "audioLevelsControl";
            this.audioLevelsControl.Size = new System.Drawing.Size(331, 257);
            this.audioLevelsControl.TabIndex = 0;
            this.audioLevelsControl.Text = "audioLevelsUIControl1";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.ClientSize = new System.Drawing.Size(352, 379);
            this.Controls.Add(this.tbPercent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.updownRestoreDelay);
            this.Controls.Add(this.tbProcessName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.updownSamplingDelay);
            this.Controls.Add(this.btnChooseProcess);
            this.Controls.Add(this.audioLevelsControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Windows Volume Limiter";
            ((System.ComponentModel.ISupportInitialize)(this.updownSamplingDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownRestoreDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AudioLevelsUIControl audioLevelsControl;
        private System.Windows.Forms.Button btnChooseProcess;
        private System.Windows.Forms.NumericUpDown updownSamplingDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbProcessName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown updownRestoreDelay;
        private System.Windows.Forms.TrackBar tbPercent;
    }
}

