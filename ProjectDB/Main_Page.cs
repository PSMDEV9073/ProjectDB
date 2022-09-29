using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;

namespace ProjectDB
{
    public partial class Main_Page : Form
    {
        private PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes");
        string process_name = Process.GetCurrentProcess().ProcessName;
        private PerformanceCounter prcess_cpu = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        private bool loop_state = true;
        System.Threading.Timer timer;

        private int number = 0;

        public Main_Page()
        {
            InitializeComponent();

            panel3.Hide();
            panel5.Hide();

            this.loading_timer.Enabled = true;
            this.loading_timer.Tick += loading_timer_Tick;

            roundBorderPanel1.isBorder = false;
            roundBorderPanel1.isFill = true;
            roundBorderPanel1.radius = 20;
            roundBorderPanel1.fillColor = Color.FromArgb(15, 139, 227);


            roundBorderPanel2.isBorder = false;
            roundBorderPanel2.isFill = true;
            roundBorderPanel2.radius = 20;
            roundBorderPanel2.fillColor = Color.FromArgb(100, 24, 195);


            roundBorderPanel3.isBorder = false;
            roundBorderPanel3.isFill = true;
            roundBorderPanel3.radius = 20;
            roundBorderPanel3.fillColor = Color.FromArgb(254, 124, 49);


            if (timer != null)
            {
                timer.Change(2000, System.Threading.Timeout.Infinite);
            }
            else
            {
                timer = new System.Threading.Timer(obj => {
                    Thread my_thread = new Thread(check_system);
                    my_thread.Start();
                    runnig_programs();
                }, null, 3000, System.Threading.Timeout.Infinite);
            }
        }

        private void Main_Page_Load(object sender, EventArgs e)
        {
        }

        private void loading_timer_Tick(object sender, EventArgs e)
        {
            number++;

            if (number > 100)
            {
                this.loading_timer.Enabled = false;
                panel3.Show();
                panel5.Show();
                system_information();
                return;
            }

            this.loading_progressBar.Value = number;


            if (loading_progressBar.Value == 100)
            {
                loading_timer.Enabled = false;
                panel3.Show();
                panel5.Show();
                system_information();
            }
            else { loading_progressBar.Value += 1; }
        }

        private void system_information()
        {
            // 1. CPU 정보 가져오기
            ManagementObjectSearcher MS2 = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject MO in MS2.Get())
            {
                lbl_cpu_name.Text = "CPU: " + MO["Name"].ToString();
            }

            // 2. 램 정보 가져오기
            ulong a = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            lbl_ram_name.Text = "RAM: " + FormatBytes((long)a);
        }

        private void check_system()
        {
            do
            {
                if (this.InvokeRequired)
                {
                    this.lbl_cpu.BeginInvoke(new Action(() =>
                    {
                        this.lbl_cpu.Text = cpu.NextValue().ToString() + " %";
                        this.lbl_ram.Text = ram.NextValue().ToString() + " MB";
                        this.lbl_cpu2.Text = prcess_cpu.NextValue().ToString() + " %";
                        this.lbl_cpu_project.Text = process_name + " CPU 사용량";
                        this.lbl_project_name.Text = process_name;
                    }));
                }
                else
                {
                    this.lbl_cpu.Text = cpu.NextValue().ToString() + " %";
                    this.lbl_ram.Text = ram.NextValue().ToString() + " MB";
                    this.lbl_cpu2.Text = prcess_cpu.NextValue().ToString() + " %";
                    this.lbl_cpu_project.Text = process_name + " CPU 사용량";
                    this.lbl_project_name.Text = process_name;
                }
                Thread.Sleep(1000);
            } while (loop_state);
        }

        private void runnig_programs()
        {
            Process[] processes = Process.GetProcesses();
            var prcs = Process.GetCurrentProcess();

            DataTable table = new DataTable();
            // column을 추가합니다.
            table.Columns.Add("No", typeof(string));
            table.Columns.Add("이름", typeof(string));
            table.Columns.Add("메모리", typeof(string));

            foreach (Process p in processes)
            {
                // 각각의 행에 내용을 입력합니다.
                table.Rows.Add(p.Id, p.ProcessName, FormatBytes(p.VirtualMemorySize));
                // 값들이 입력된 테이블을 DataGridView에 입력합니다.
                dataGridView1.DataSource = table;
            }
        }

        private void Main_Page_FormClosing(object sender, FormClosingEventArgs e)
        {
            loop_state = false;  // for worker thread exit....
        }

        public string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
    }
}