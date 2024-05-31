using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using ZXing;
using System.IO;
using System.Diagnostics;
using Emgu.CV.CvEnum;
using System.Collections.Generic;
using Emgu.CV.Util;
using Microsoft.VisualBasic.Devices;
using System.Linq;
using System.Threading;
using System.Reflection;

namespace camera_show_denali {
    public partial class Form1 : Form {
        private VideoCapture capture = null;
        private VideoCapture.API captureApi = VideoCapture.API.Any;
        private Image<Bgr, Byte> img;

        public Form1() {
            InitializeComponent();
            ComputerInfo computerInfo = new ComputerInfo();
            if (!computerInfo.OSFullName.Contains("Windows 7"))
            {
                captureApi = VideoCapture.API.DShow;
            }
            string flag_set_port = string.Empty;
            try
            { 
                flag_set_port = File.ReadAllText("set_port.txt");
            } catch { }
            if (flag_set_port == "set port") {
                File.Delete("set_port.txt");
                Form f2 = new Form();
                f2.Size = new Size(100, 100);
                ComboBox c = new ComboBox();
                c.Size = new Size(60, 7);
                for (int i = 0; i < 9; i++) {
                    capture = new VideoCapture(i, captureApi);
                    if (capture.Width != 0) c.Items.Add(i);
                    capture.Dispose();
                }
                f2.Controls.Add(c);
                f2.ShowDialog();
                capture = new VideoCapture(Convert.ToInt32(c.Text), captureApi);
            } else {
                capture = new VideoCapture(0, captureApi);
            }
            capture.SetCaptureProperty(CapProp.FrameHeight, 800);//800
            capture.SetCaptureProperty(CapProp.FrameWidth, 600);//600
            this.Size = new Size(capture.Width, capture.Height);
            pictureBox1.Size = new Size(capture.Width, capture.Height);
            Application.Idle += check_led;
        }
        private void Form1_Load(object sender, EventArgs e) {

        }
        private void check_led(object sender, EventArgs e) {
            Mat frame;
            try {
                frame = capture.QueryFrame();
                img = frame.ToImage<Bgr, Byte>();
            } catch {
                MessageBox.Show("ไม่สามารถเปิดกล้องได้");
                Application.Exit();
                return;
            }
            pictureBox1.Image = img.Bitmap;
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }
        private static void DelaymS(int mS) {
            Stopwatch stopwatchDelaymS = new Stopwatch();
            stopwatchDelaymS.Restart();
            while (mS > stopwatchDelaymS.ElapsedMilliseconds) {
                if (!stopwatchDelaymS.IsRunning)
                    stopwatchDelaymS.Start();
                Application.DoEvents();
            }
            stopwatchDelaymS.Stop();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            //capture.Dispose();
        }
        private void setPortToolStripMenuItem_Click(object sender, EventArgs e) {
            File.WriteAllText("set_port.txt", "set port");
            capture.Dispose();
            Application.Restart();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            try { capture.Dispose(); } catch { }
        }
        private void ctms_propertySetting_Click(object sender, EventArgs e) {
            capture.SetCaptureProperty(CapProp.Settings, 0);
        }
    }
}
