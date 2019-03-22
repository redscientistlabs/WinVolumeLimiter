using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace WinVolumeLimiter
{
    public partial class SelectProcess : Form
    {

        public string SelectedProcess = String.Empty;


        public SelectProcess()
        {
            InitializeComponent();
        }


        private void WGH_HookToProcess_Load(object sender, EventArgs e)
        {
            lbProcesses.Items.AddRange(Process.GetProcesses().Select(it => $"{it.ProcessName}").Distinct().OrderBy(x => x).ToArray());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

		private void btnSendList_Click(object sender, EventArgs e)
		{

			if (lbProcesses.SelectedIndex == -1)
			{
				MessageBox.Show("There's no process selected");
				return;
			}

            SelectedProcess = lbProcesses.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
            
        }
    }
}
