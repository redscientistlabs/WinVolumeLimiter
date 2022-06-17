//Based on https://github.com/jeske/SoundLevelMonitor

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinVolumeLimiter
{
    public sealed partial class MainForm : Form
    {
        private string version = "1.3";
        AudioLevelMonitor audioMonitor;
        private bool updating = false;
        private bool initialized = false;
        public MainForm()
        {
            InitializeComponent();
            audioMonitor = new AudioLevelMonitor("");
            updownSamplingDelay.ValueChanged += UpdownSamplingDelay_ValueChanged;
            updownRestoreDelay.ValueChanged += UpdownRestoreDelay_ValueChanged;
            tbMonitorVolume.ValueChanged += tbMonitorVolume_ValueChanged;
            tbDuckingVolume.ValueChanged += tbDuckingVolume_ValueChanged;
            this.DragDrop += MainForm_DragDrop;
            this.DragEnter += MainForm_DragEnter;
            tbMasterVolume.ValueChanged += tbMasterVolume_ValueChanged;

            this.FormClosing += (o, e) => audioMonitor?.Stop();
            this.Text = $"Windows Volume Limiter v{version}";

            initialized = true;
            //Set up the actual volume and have it propogate
            tbMonitorVolume_ValueChanged(null,null);


        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                var file = files[0];
                if (file.EndsWith(".exe",StringComparison.OrdinalIgnoreCase))
                {
                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = file;
                    p.WorkingDirectory = Path.GetDirectoryName(file) ?? throw new InvalidOperationException();
                    Process.Start(p);
                    StartNewMonitor(Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }


        private void UpdownRestoreDelay_ValueChanged(object sender, EventArgs e)
        {
            audioMonitor.RestoreDelay = Convert.ToInt32(updownRestoreDelay.Value);
        }

        private void UpdownSamplingDelay_ValueChanged(object sender, EventArgs e)
        {
            audioMonitor.Intervalms = Convert.ToInt32(updownSamplingDelay.Value);
        }

        private void tbMonitorVolume_ValueChanged(object sender, EventArgs e)
        {
            if (!initialized)
                return;

            audioMonitor.MonitorVolume = tbMonitorVolume.Value * .01f;
            audioMonitor.Ducked = false;

            if (!updating && cbLinked.Checked)
            {
                var value = Convert.ToInt32((tbMonitorVolume.Value * updownLinkRatio.Value));
                if (value > tbDuckingVolume.Maximum)
                    value = tbDuckingVolume.Maximum;

                if (value < tbDuckingVolume.Minimum)
                    value = tbDuckingVolume.Minimum;

                updating = true;
                tbDuckingVolume.Value = value;
            }
            updating = false;
        }
        private void tbDuckingVolume_ValueChanged(object sender, EventArgs e)
        {
            if (!initialized)
                return;

            audioMonitor.DuckingVolume = tbDuckingVolume.Value * .01f;
            audioMonitor.Ducked = false;

            if (!updating && cbLinked.Checked)
            {
                var value = Convert.ToInt32((tbDuckingVolume.Value / updownLinkRatio.Value));

                if (value > tbMonitorVolume.Maximum)
                    value = tbMonitorVolume.Maximum;

                if (value < tbMonitorVolume.Minimum)
                    value = tbMonitorVolume.Minimum;

                updating = true;
                tbMonitorVolume.Value = value;
            }
            updating = false;
        }

        private void tbMasterVolume_ValueChanged(object sender, EventArgs e)
        {
            if(audioMonitor != null)
                audioMonitor.MasterVolume = tbMasterVolume.Value * .01f;
        }

        private void BtnChooseProcess_Click(object sender, EventArgs e)
        {
            var selectProcess = new SelectProcess();
            DialogResult dr = selectProcess.ShowDialog();
            if (dr == DialogResult.OK)
                StartNewMonitor(selectProcess.SelectedProcess);
        }

        private void StartNewMonitor(string processName)
        {
            initialized = false;
            tbProcessName.Text = processName;
            audioMonitor.Stop();
            audioMonitor = new AudioLevelMonitor(processName);
            audioMonitor.RestoreDelay = Convert.ToInt32(updownRestoreDelay.Value);
            audioMonitor.Intervalms = Convert.ToInt32(updownSamplingDelay.Value);
            tbMasterVolume.Value = Convert.ToInt32(audioMonitor.MasterVolume * 100);
            initialized = true;
            //Set up the actual volume and have it propogate
            tbMonitorVolume_ValueChanged(null, null);
            tbDuckingVolume_ValueChanged(null, null);
            audioLevelsControl.AudioMonitor = audioMonitor;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void CbLinked_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnabled.Checked)
            {
                audioMonitor?.Start();
            }
            else
                audioMonitor?.Stop();
        }
    }

    public class PanelCustomBorder : Panel
    {
        [CategoryAttribute("Appearance"), DescriptionAttribute("Sets the border color")]
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        public Color BorderColor { get; set; }

        [CategoryAttribute("Appearance"), DescriptionAttribute("Sets the border width")]
        [DefaultValue(2)]
        public int BorderWidth { get; set; }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(
                new Pen(new SolidBrush(BorderColor), BorderWidth),
                e.ClipRectangle);
        }

    }
}
