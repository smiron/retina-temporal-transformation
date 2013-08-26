using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace RetinaViewer
{
    public partial class MainWindow : Form
    {
        private Capture _capture;
        private Retina _retina;
        private const int RetinaSizeX = 210;
        private const int RetinaSizeY = 104;
        private bool _captureInProgress;

        public MainWindow()
        {
            InitializeComponent();
            Load += MainWindowLoad;
            Closing += MainWindowClosing;
        }

        void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _capture.Stop();
            _capture.Dispose();
        }

        void MainWindowLoad(object sender, EventArgs e)
        {
            _retina = new Retina(new Size(RetinaSizeX, RetinaSizeY));

            var parvoParam = _retina.Parameters.OPLandIplParvo;
            parvoParam.GanglionCellsSensitivity = 0.89f;
            parvoParam.HorizontalCellsGain = 0.3f;

            _retina.Parameters = new Retina.RetinaParameters
            {
                IplMagno = _retina.Parameters.IplMagno,
                OPLandIplParvo = parvoParam
            };

            _retina.ClearBuffers();
        }

        private void StartCaptureButton(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {
                    _capture.Pause(); //Pause the capture
                    _captureInProgress = false; //Flag the state of the camera
                }
                else
                {
                    _capture.Start(); //Start the capture
                    _captureInProgress = true; //Flag the state of the camera
                }

            }
            else
            {
                SetupCapture();
                StartCaptureButton(null, null);
            }
        }


        private void SetupCapture()
        {
            //Dispose of Capture if it was created before
            if (_capture != null) _capture.Dispose();
            try
            {
                //Set up capture device
                _capture = new Capture(0);
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            Image<Bgr, Byte> frame = _capture.RetrieveBgrFrame();

            var cropRect = new Rectangle(0, 0, RetinaSizeX, RetinaSizeY);
            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(frame.Bitmap, new Rectangle(0, 0, target.Width, target.Height),
                            new Rectangle(0, 0, frame.Bitmap.Width, frame.Bitmap.Height),
                            GraphicsUnit.Pixel);
            }

            var original = new Image<Bgr, byte>(target);
            
            _retina.Run(original);
            
            
            var parvo = _retina.GetParvo();

            var grayParvo = parvo.Convert<Gray, byte>().Convert<Bgr, byte>();

            var mango = _retina.GetMagno().Convert<Bgr, byte>();

            var r = original.ConcateVertical(parvo).ConcateVertical(grayParvo).ConcateVertical(mango);

            //because we are using an autosize picturebox we need to do a thread safe update
            DisplayImage(r.ToBitmap());
        }


        private delegate void DisplayImageDelegate(Bitmap image);
        private void DisplayImage(Bitmap image)
        {
            if (captureImage.InvokeRequired)
            {
                try
                {
                    var di = new DisplayImageDelegate(DisplayImage);
                    BeginInvoke(di, new object[] { image });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                captureImage.Image = image;
            }
        }
    }
}
