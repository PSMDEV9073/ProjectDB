using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.IO;
using System.Collections.Specialized;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace ProjectDB
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 25, 25));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                      , int nTopRect
                                                      , int nRightRect
                                                      , int nBottomRect
                                                      , int nWidthEllipse
                                                      , int nHeightEllipse);

        private void Main_Load(object sender, EventArgs e)
        {
            string WindowsAccount = Environment.UserName;
            user_name.Text = WindowsAccount;

            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                user_ip.Text = localIP;
            }

            openChildForm(new Main_Page());

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            try
            {
                int storCount = allDrives.Count();
                label4.Text = "";
                //MessageBox.Show(string.Format("Drive Count : {0}", storCount));

                foreach (DriveInfo d in allDrives)
                {
                    if (d.IsReady == true)
                    {
                        Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                        Console.WriteLine("  File system: {0}", d.DriveFormat);
                        Console.WriteLine("  Available space to current user:{0, 15} bytes", d.AvailableFreeSpace);

                        Console.WriteLine("  Total available space:          {0, 15} bytes", d.TotalFreeSpace);

                        Console.WriteLine("  Total size of drive:            {0, 15} bytes ", d.TotalSize);

                        string msg;
                        msg = string.Format("{0}\n{1}/{2}\n",
                            d.Name,
                            FormatBytes(d.TotalSize),
                            FormatBytes(d.TotalFreeSpace));

                        //msg = string.Format("{0}\n{1}\n{2}\n", d.Name, d.TotalSize,d.TotalFreeSpace);
                        label4.Text += msg;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "TB", "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void hide_btn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private Point mousePoint;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X),
                     this.Top - (mousePoint.Y - e.Y));
            }
        }

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null) activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void main_page_btn_Click(object sender, EventArgs e)
        {
            openChildForm(new Main_Page());
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}