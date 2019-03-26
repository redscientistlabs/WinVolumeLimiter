//Based on https://github.com/jeske/SoundLevelMonitor

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinVolumeLimiter
{
    public partial class MainForm : Form
    {
        AudioLevelMonitor audioMonitor;

        public MainForm()
        {
            InitializeComponent();
            audioMonitor = new AudioLevelMonitor();
            audioLevelsControl.AudioMonitor = audioMonitor;
            tbProcessName.DataBindings.Add("Text",
                                this.audioMonitor,
                                "ProcessName",
                                false,
                                DataSourceUpdateMode.OnPropertyChanged);
            updownSamplingDelay.DataBindings.Add("Value",
                                this.audioMonitor,
                                "interval_ms",
                                false,
                                DataSourceUpdateMode.OnPropertyChanged);
            tbPercent.ValueChanged += TbPercent_ValueChanged;
        }

        private void TbPercent_ValueChanged(object sender, EventArgs e)
        {
            audioMonitor.MaxVolume = tbPercent.Value * .01f;
            if (audioMonitor.Ducked)
            {
                audioMonitor.Ducked = false;
                audioMonitor.DontChangeMax = true;
            }                
        }

        private void BtnChooseProcess_Click(object sender, EventArgs e)
        {
            var selectProcess = new SelectProcess();
            DialogResult dr = selectProcess.ShowDialog();
            if(dr == DialogResult.OK)
            {
                tbProcessName.Text = selectProcess.SelectedProcess;
            }
        }
    }
}
