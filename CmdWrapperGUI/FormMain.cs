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

        bool startOK = false;

        public FormMain()
        {
            InitializeComponent();

            // 启动cmd.exe进程
            p = new Process();
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
            


            // 界面按钮互锁
            button2.Enabled = false;
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
                try
                {
                    Invoke(new updateDelegate(update), new object[] { msg });
                } catch(Exception e)
                {
                    // not to do
                }
                
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
            p.StartInfo.FileName = textBox1.Text;
            p.StartInfo.Arguments = textBox3.Text;
            startOK = p.Start();
            if(startOK)
            {
                // 重定向输入
                input = p.StandardInput;
                // 开始监控输出（异步读取）
                p.BeginOutputReadLine();

                // input.WriteLine(textBox1.Text);
                // textBox1.SelectAll();

                // 界面按钮互锁
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //p.Kill();
            //p.Close();

            //try
            //{
            //    // 获得指定进程
            //    Process[] p = Process.GetProcessesByName("brook_windows_amd64.exe");
            //    // 杀死该进程
            //    p[0].Kill(); 
            //    MessageBox.Show("进程关闭成功！");
            //}
            //catch
            //{
            //    MessageBox.Show("无法关闭此进程！");
            //}
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 当用户点击窗体右上角X按钮或(Alt+F4)时发生         
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.ShowInTaskbar = false;
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
            // 结束启动的进程
            if (startOK)
            {
                p.Kill();
                p.Close();
            }

            // 整个程序退出
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (startOK)
            {
                p.Kill();
                p.Close();
            }
            button1.Enabled = true;
            button2.Enabled = false;

        }
    }
}
