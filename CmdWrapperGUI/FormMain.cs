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
            // 自定义shell
            p.StartInfo.UseShellExecute = false;
            // 避免显示原始窗口
            p.StartInfo.CreateNoWindow = true;
            // 重定向标准输入（原来是CON）
            p.StartInfo.RedirectStandardInput = true;
            // 重定向标准输出
            p.StartInfo.RedirectStandardOutput = true;
            // 数据接收事件（标准输出重定向至此）
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

            p.Start();
            // 重定向输入
            input = p.StandardInput;
            // 开始监控输出（异步读取）
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
            {
                Invoke(new updateDelegate(update), new object[] { msg });
            }
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
            p.Kill();
            p.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 当用户点击窗体右上角X按钮或(Alt + F4)时发生         
            if (e.CloseReason == CloseReason.UserClosing) 
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                // this.myIcon.Icon = this.Icon;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Control.MousePosition);
            }

            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
