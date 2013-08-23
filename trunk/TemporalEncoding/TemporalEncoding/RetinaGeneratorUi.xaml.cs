using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;

using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace TemporalEncoding
{
    public partial class RetinaGeneratorUi
    {

        private const int RetinaSize = 9;
        private Retina _retina;
        private const int DisplaySize = 250 / RetinaSize;
        private const int DisplaySize1 = 70;
        DispatcherTimer _timer;

        public RetinaGeneratorUi()
        {
            InitializeComponent();
            Loaded += RetinaGeneratorUiLoaded;
        }

        void RetinaGeneratorUiLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _retina = new Retina(RetinaSize, 1, 3);

            GenerateUiObjects(_canvas, _retina.PhotoreceptorsVoltage.GetLength(0));
            GenerateUiObjects(_offCanvas, _retina.OnBipolarsVoltage.GetLength(0));
            GenerateUiObjects(_onCanvas, _retina.OffBipolarsVoltage.GetLength(0));
            GenerateUiObjects(_onFrequencyCanvas, _retina.OnMigetGanglionFrequency.GetLength(0));
            GenerateUiObjects(_offFrequencyCanvas, _retina.OffMigetGanglionFrequency.GetLength(0));
            GenerateOneDUiObjects(_onSpikeCanvas, _retina.OnMigetGanglionVoltage.GetLength(0));
            GenerateOneDUiObjects(_offSpikeCanvas, _retina.OffMigetGanglionVoltage.GetLength(0));

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _timer.Tick += TimerTick;
            _timer.IsEnabled = true;

            Draw();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _retina.IterateTime();

            for (int i = 0; i < _retina.OnMigetGanglionActionPotentials.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OnMigetGanglionActionPotentials.GetLength(1); j++)
                {
                    var index = i + j * _retina.OnMigetGanglionActionPotentials.GetLength(0);

                    ((Rectangle)_onSpikeCanvas.Children[index]).Fill = _retina.OnMigetGanglionActionPotentials[i, j] ?
                        new SolidColorBrush(Colors.DarkBlue) :
                        new SolidColorBrush(Colors.White);
                }
            }

            for (int i = 0; i < _retina.OffMigetGanglionActionPotentials.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OffMigetGanglionActionPotentials.GetLength(1); j++)
                {
                    var index = i + j * _retina.OffMigetGanglionActionPotentials.GetLength(0);

                    ((Rectangle)_offSpikeCanvas.Children[index]).Fill = _retina.OffMigetGanglionActionPotentials[i, j] ?
                        new SolidColorBrush(Colors.DarkBlue) :
                        new SolidColorBrush(Colors.White);
                }
            }
        }

        private void Draw()
        {
            for (int i = 0; i < _retina.PhotoreceptorsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.PhotoreceptorsVoltage.GetLength(1); j++)
                {
                    var index = i + j * _retina.PhotoreceptorsVoltage.GetLength(0);

                    ((Rectangle)_canvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(
                        _retina.PhotoreceptorsVoltage[i, j],
                        _retina.PhotoreceptorsVoltage[i, j],
                        _retina.PhotoreceptorsVoltage[i, j]));

                }
            }

            for (int i = 0; i < _retina.OnBipolarsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OnBipolarsVoltage.GetLength(1); j++)
                {
                    var index = i + j * _retina.OnBipolarsVoltage.GetLength(0);

                    ((Rectangle)_onCanvas.Children[index]).Fill = _retina.OnBipolarsVoltage[i, j] >= 0 ?
                        new SolidColorBrush(Color.FromRgb((byte)_retina.OnBipolarsVoltage[i, j], 0, 0)) :
                        new SolidColorBrush(Color.FromRgb(0, 0, (byte)Math.Abs(_retina.OnBipolarsVoltage[i, j])));
                }
            }

            for (int i = 0; i < _retina.OffBipolarsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OffBipolarsVoltage.GetLength(1); j++)
                {
                    var index = i + j * _retina.OffBipolarsVoltage.GetLength(0);

                    ((Rectangle)_offCanvas.Children[index]).Fill = _retina.OffBipolarsVoltage[i, j] >= 0 ?
                        new SolidColorBrush(Color.FromRgb((byte)_retina.OffBipolarsVoltage[i, j], 0, 0)) :
                        new SolidColorBrush(Color.FromRgb(0, 0, (byte)Math.Abs(_retina.OffBipolarsVoltage[i, j])));
                }
            }




            for (int i = 0; i < _retina.OnMigetGanglionFrequency.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OnMigetGanglionFrequency.GetLength(1); j++)
                {
                    var index = i + j * _retina.OnMigetGanglionFrequency.GetLength(0);

                    var color = (byte)(_retina.OffMigetGanglionFrequency[i, j]);
                    ((Rectangle)_onFrequencyCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(color, color, color));
                }
            }

            for (int i = 0; i < _retina.OffMigetGanglionFrequency.GetLength(0); i++)
            {
                for (int j = 0; j < _retina.OffMigetGanglionFrequency.GetLength(1); j++)
                {
                    var index = i + j * _retina.OffMigetGanglionFrequency.GetLength(0);
                    var color = (byte)(_retina.OffMigetGanglionFrequency[i, j]);
                    ((Rectangle)_offFrequencyCanvas.Children[index]).Fill = new SolidColorBrush(Color.FromRgb(color, color, color));
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


        private void GenerateOneDUiObjects(Canvas canvas, int length)
        {
            for (int i = 0; i < length * length; i++)
            {

                var pixel = new Rectangle
                {
                    Width = DisplaySize1,
                    Height = 8,
                    Fill = new SolidColorBrush { Color = Colors.White }
                };
                Canvas.SetLeft(pixel, i * DisplaySize1);
                canvas.Children.Add(pixel);

            }
        }


        private void Button1Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _retina.ChangeRetinaImage();
            Draw();
        }

        private void Button2Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _retina.ChangeRetinaImage(@"C:\temp\temporal\TemporalEncoding\TemporalEncoding\Images\Test1.JPG");
            Draw();
        }
    }
}
