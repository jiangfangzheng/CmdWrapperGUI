using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmdWrapperGUI
{
    public partial class FormMain : Form
    {

        Process p;
        StreamWriter input;

        public FormMain()
        {
            InitializeComponent();

            // 启动cmd.exe进程
            p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

            p.Start();
            input = p.StandardInput;
            p.BeginOutputReadLine();
        }

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            update(e.Data + Environment.NewLine);
        }

        delegate void updateDelegate(string msg);
        void update(string msg)
        {
            if (this.InvokeRequired)
                Invoke(new updateDelegate(update), new object[] { msg });
            else
            {
                textBox2.Text += msg;
                textBox2.SelectionStart = textBox2.Text.Length - 1;
                textBox2.ScrollToCaret();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            input.WriteLine(textBox1.Text);
            textBox1.SelectAll();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            p.Close();
        }
    }
}
