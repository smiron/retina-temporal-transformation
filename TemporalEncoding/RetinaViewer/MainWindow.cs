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
        private const int RetinaSizeX = 200;
        private const int RetinaSizeY = 100;
        private bool _captureInProgress;

        public MainWindow()
        {
            InitializeComponent();
            Load += MainWindowLoad;
        }

        void MainWindowLoad(object sender, EventArgs e)
        {
            _retina = new Retina(new Size(RetinaSizeX, RetinaSizeY));
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
            
            _retina.Run(new Image<Bgr, byte>(target));


            var aa = new Image<Bgr, byte>(target);
            

            var result = aa.ConcateVertical(_retina.GetParvo());

            var r =result.ConcateVertical(_retina.GetMagno().Convert<Bgr, byte>());


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
