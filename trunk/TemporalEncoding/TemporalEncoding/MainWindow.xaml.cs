using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TemporalEncoding
{
    public partial class MainWindow
    {

        private Random _ran = new Random();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private Rectangle GenerateRectangle()
        {
            var color = (byte)_ran.Next(256);
            return new Rectangle
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush { Color = Color.FromRgb(color, color, color) }
            };
        }



        void MainWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    var pixel = GenerateRectangle();
                    Canvas.SetLeft(pixel, j * 5);
                    Canvas.SetTop(pixel, i * 5);
                    _canvas.Children.Add(pixel);
                }
            }
        }


        private void LoadButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //var op = new OpenFileDialog
            //{
            //    Title = "Select a picture",
            //    Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            //             "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            //             "Portable Network Graphic (*.png)|*.png"
            //};
            //if (op.ShowDialog() == true)
            //{
            //    _sourceImage.Source = new BitmapImage(new Uri(op.FileName));
            //}


        }
    }
}
