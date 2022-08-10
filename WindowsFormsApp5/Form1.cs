using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Accord.Imaging;
using Accord.DataSets;
using Accord.Vision.Detection;
using Accord.Imaging.Filters;
using Accord.Video.DirectShow;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public int SelectCaptureDevice = -1;
        public Accord.Video.DirectShow.VideoCaptureDevice VideoCapture;
        public Accord.Video.DirectShow.VideoCaptureDevice VideoCapture2;

        public Grayscale grayscale = Grayscale.CommonAlgorithms.BT709;
        public Bitmap kkk;
        // Accord.Imaging.Filters.Subtract subtract = new Subtract();
        // Invert subtract = new Invert();
        Accord.Imaging.Filters.Difference subtract = new Difference();

        Crop crop;

        public Form1()
        {
            InitializeComponent();
        }
     
        private void video_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
         
            if (VideoCapture.IsRunning)
            {

                Bitmap Render = new Bitmap(eventArgs.Frame.Width, eventArgs.Frame.Height, PixelFormat.Format32bppArgb);
                var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();
                var detector = new HaarObjectDetector(cascade, minSize: 150, searchMode: ObjectDetectorSearchMode.NoOverlap);
                Rectangle[] rectangles = detector.ProcessFrame(eventArgs.Frame);
                Graphics p = Graphics.FromImage(Render);
                if (rectangles != null)
                {
                    try
                    {

                        p.DrawImage(eventArgs.Frame, new Point(0, 0));
                        p.DrawRectangles(new Pen(Color.Red, 5F), rectangles);

                    }
                    catch { }
                }
                pictureBox1.Image = Render;
                p.Dispose();


            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var videoDevices = new Accord.Video.DirectShow.FilterInfoCollection(Accord.Video.DirectShow.FilterCategory.VideoInputDevice);
            VideoCapture = new Accord.Video.DirectShow.VideoCaptureDevice(videoDevices[SelectCaptureDevice].MonikerString);
            VideoCapture.NewFrame += new Accord.Video.NewFrameEventHandler(video_NewFrame);
            VideoCapture.Start();
        

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count <= 0) { return; }
            SelectCaptureDevice = comboBox1.SelectedIndex;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            var videoDevices = new Accord.Video.DirectShow.FilterInfoCollection(Accord.Video.DirectShow.FilterCategory.VideoInputDevice);
            for (int i = 0; i < videoDevices.Count; i++)
            {
                comboBox1.Items.Add(videoDevices[i].Name);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VideoCapture.Stop(); Application.ExitThread();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((VideoCapture != null) && (VideoCapture is VideoCaptureDevice))
            {
                try
                {
                    ((VideoCaptureDevice)VideoCapture).DisplayPropertyPage(this.Handle);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("The video source does not support configuration property page.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
