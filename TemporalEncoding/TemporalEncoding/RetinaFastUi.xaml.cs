using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TemporalEncoding
{
    public partial class RetinaFastUi
    {

        private const string ImgPath = @"C:\temp\temporal\TemporalEncoding\TemporalEncoding\Images\Test1.JPG";
        private const int RetinaSize = 300;
        private static readonly Random Ran = new Random();
        DispatcherTimer _timer;
        private Retina _retina;

        public RetinaFastUi()
        {
            InitializeComponent();
            Loaded += RetinaFastUiLoaded;
        }

        private void RetinaFastUiLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _retina = new Retina(RetinaSize, 1, 7);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            _timer.Tick += TimerTick;
            _timer.IsEnabled = true;
        }

        private int _counter = 0;

        private void TimerTick(object sender, EventArgs e)
        {

            if (_counter == 20)
            {
                _counter = 0;
                ChangeRetinaImage();
            }

            _retina.IterateTime();

            var target = new Bitmap(_retina.OnMigetGanglionActionPotentials.GetLength(0), _retina.OnMigetGanglionActionPotentials.GetLength(1));

            for (int i = 0; i < _retina.OnMigetGanglionActionPotentials.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OnMigetGanglionActionPotentials.GetLength(1); j++)
                {
                    if (_retina.OnMigetGanglionActionPotentials[i, j])
                    {
                        target.SetPixel(i, j, Color.DarkBlue);
                    }
                    else
                    {
                        target.SetPixel(i, j, Color.White);
                    }
                }
            }

            _onMidgetGanglions.Source = ConvertToBitmapImage(target);

            _counter++;
        }

        private void LoadButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeRetinaImage();
        }

        private void ChangeRetinaImage()
        {
            var src = Image.FromFile(ImgPath) as Bitmap;

            if (src == null)
            {
                return;
            }

            var startX = Ran.Next(src.Width - RetinaSize);
            var startY = Ran.Next(src.Height - RetinaSize);

            var cropRect = new Rectangle(startX, startY, RetinaSize, RetinaSize);


            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                            cropRect,
                            GraphicsUnit.Pixel);
            }

            var grayImage = MakeGrayscale3(target);

            _retina.ChangeRetinaImage(grayImage);

            var bi = ConvertToBitmapImage(grayImage);

            _source.Source = bi;
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





