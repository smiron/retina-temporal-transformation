using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Emgu.CV;
using RetinaA = Emgu.CV.Retina;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace TemporalEncoding
{
    public partial class RetinaFastUi
    {

        private const string ImgPath = @"C:\temp\temporal\TemporalEncoding\TemporalEncoding\Images\Test1.JPG";
        private const int RetinaSizeX = 100;
        private const int RetinaSizeY = 100;
        private static readonly Random Ran = new Random();
        private RetinaA _retina;
        private Capture _capture; //Camera

        public RetinaFastUi()
        {
            InitializeComponent();
            Loaded += RetinaFastUiLoaded;
        }

        private void RetinaFastUiLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _retina = new Emgu.CV.Retina(new Size(RetinaSizeX, RetinaSizeY));
            _retina.ClearBuffers();
        }

        

        private void LoadButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeRetinaImage();
        }

        private void ChangeRetinaImage()
        {

           //SetupCapture();







            //var src = Image.FromFile(ImgPath) as Bitmap;

            //if (src == null)
            //{
            //    return;
            //}

            //var startX = Ran.Next(src.Width - RetinaSizeX);
            //var startY = Ran.Next(src.Height - RetinaSizeY);

            //var cropRect = new Rectangle(startX, startY, RetinaSizeX, RetinaSizeY);


            //var target = new Bitmap(cropRect.Width, cropRect.Height);

            //using (Graphics g = Graphics.FromImage(target))
            //{
            //    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
            //                cropRect,
            //                GraphicsUnit.Pixel);
            //}


            //_retina.Run(new Image<Bgr, byte>(target));

            //var parvo = _retina.GetParvo();


            //var bi = ConvertToBitmapImage(parvo.Bitmap);

            //_source.Source = bi;


            //var v = new ImageViewer();

            //using (var capture = new Capture(0))
            //using (var retina = new RetinaA(new Size(RetinaSizeX, RetinaSizeY), true, RetinaA.ColorSamplingMethod.ColorBayer, false, 1.0, 10.0))
            //{



            //    while (true)
            //    {
            //        Image<Bgr, byte> img = capture.QuerySmallFrame();


            //        var cropRect = new Rectangle(0, 0, RetinaSizeX, RetinaSizeY);
            //        var target = new Bitmap(cropRect.Width, cropRect.Height);

            //        using (Graphics g = Graphics.FromImage(target))
            //        {
            //            g.DrawImage(img.Bitmap, new Rectangle(0, 0, target.Width, target.Height),
            //                        cropRect,
            //                        GraphicsUnit.Pixel);
            //        }

            //        retina.Run(new Image<Bgr, byte>(target));

            //        v.Image = img.ConcateVertical(retina.GetParvo().ConcateHorizontal(retina.GetMagno().Convert<Bgr, byte>()));



            //        //_source.Source = ConvertToBitmapImage(v.Image.Bitmap);
            //        v.Show();

            //        Thread.Sleep(1000);

            //    }
            //}
        }

        private static BitmapImage ConvertToBitmapImage(Bitmap grayImage)
        {
            var ms = new MemoryStream();
            grayImage.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }


        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            var colorMatrix = new ColorMatrix(
                new[]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            var attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}





