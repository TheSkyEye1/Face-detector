using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace lab5_2
{
    public partial class Form1 : Form
    {
        private VideoCapture capture;
        Mat mask;
        Image<Gray, byte> gray;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void webon_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture();
            capture.ImageGrabbed += ProcessFrame;
            capture.Start();
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            var frame = new Mat();
            capture.Retrieve(frame);
            Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
            using (CascadeClassifier cascadeClassifier = new CascadeClassifier(@"C:\haarcascade_frontalface_default.xml"))
            {
                Rectangle[] facesDetected = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 10,
                new Size(20, 20));
                var copy = image.Copy();
                    if (mask != null)
                    {
                        foreach (Rectangle rect in facesDetected)
                        {
                            Image<Bgra, byte> res = copy.Convert<Bgra, byte>();
                            res.ROI = rect;
                            Image<Bgra, byte> small = mask.ToImage<Bgra, byte>().Resize(rect.Width, rect.Height, Inter.Nearest);
                            Image<Gray, byte> graymask = small.Convert<Gray, byte>();
                            CvInvoke.cvCopy(small, res, small.Split()[3]);
                            res.ROI = System.Drawing.Rectangle.Empty;
                            IMG1.Image = res;
                        }

                    }
                    else
                    {
                        foreach (Rectangle rect in facesDetected)
                        {
                            copy.Draw(rect, new Bgr(Color.Yellow), 2);
                            IMG1.Image = copy;
                        }
                    }
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "(*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif;*.png";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                mask = CvInvoke.Imread(fileName, ImreadModes.Unchanged);
                gray = mask.ToImage<Gray, byte>();
                IMG1.Image = gray;
            }
        }
    }
}
