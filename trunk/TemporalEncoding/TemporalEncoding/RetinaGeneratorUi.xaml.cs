using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
namespace TemporalEncoding
{
    public partial class RetinaGeneratorUi
    {

        private const int retinaSize = 50;
        RetinaConverter retina;
        private const int DisplaySize = 250/retinaSize;

        public RetinaGeneratorUi()
        {
            InitializeComponent();
            Loaded += RetinaGeneratorUi_Loaded;
        }

        void RetinaGeneratorUi_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            retina = new RetinaConverter(retinaSize,1,7);

            GenerateUiObjects(_canvas, retina.PhotoreceptorsVoltage.GetLength(0));
            GenerateUiObjects(_offCanvas, retina.OnBipolarsVoltage.GetLength(0));
            GenerateUiObjects(_onCanvas, retina.OffBipolarsVoltage.GetLength(0));
                      

            var mat = retina.GetSpikeMatrix();
            
            Draw();
        }

        private void Draw()
        {
            for (int i = 0; i < retina.PhotoreceptorsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < retina.PhotoreceptorsVoltage.GetLength(1); j++)
                {
                    var index = i + j * retina.PhotoreceptorsVoltage.GetLength(0);

                    ((Rectangle)_canvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(
                        retina.PhotoreceptorsVoltage[i, j],
                        retina.PhotoreceptorsVoltage[i, j],
                        retina.PhotoreceptorsVoltage[i, j]));

                }
            }

            for (int i = 0; i < retina.OnBipolarsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < retina.OnBipolarsVoltage.GetLength(1); j++)
                {
                    var index = i + j * retina.OnBipolarsVoltage.GetLength(0);

                    if (retina.OnBipolarsVoltage[i, j] >= 0)
                    {
                        ((Rectangle)_onCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb((byte)retina.OnBipolarsVoltage[i, j], 0, 0));
                    }
                    else
                    {
                        ((Rectangle)_onCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(0, 0, (byte)System.Math.Abs(retina.OnBipolarsVoltage[i, j])));
                    }

                }
            }

            for (int i = 0; i < retina.OffBipolarsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < retina.OffBipolarsVoltage.GetLength(1); j++)
                {
                    var index = i + j * retina.OffBipolarsVoltage.GetLength(0);

                    if (retina.OffBipolarsVoltage[i, j] >= 0)
                    {
                        ((Rectangle)_offCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb((byte)retina.OffBipolarsVoltage[i, j], 0, 0));
                    }
                    else
                    {
                        ((Rectangle)_offCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(0, 0, (byte)System.Math.Abs(retina.OffBipolarsVoltage[i, j])));
                    }

                }
            }
        }

        private void GenerateUiObjects(Canvas canvas, int length)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    var pixel = new Rectangle
                    {
                        Width = DisplaySize,
                        Height = DisplaySize,
                        Fill = new SolidColorBrush { Color = Colors.White }
                    };
                    Canvas.SetLeft(pixel, j * DisplaySize);
                    Canvas.SetTop(pixel, i * DisplaySize);
                    canvas.Children.Add(pixel);
                }
            }
        }   

        private void button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            retina.NewFrame();
            Draw();
        }

        private void button2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            retina.NewFrame(@"D:\Test1.JPG");
            Draw();
        }
    }
}
