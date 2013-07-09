using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Righthand.RealtimeGraph;

namespace TemporalEncoding
{
    public partial class MainWindow
    {

        private readonly Random _ran = new Random();
        private const int InputSize = 5;
        private const int DisplaySize = 50;
        private const int ObservedPixels = 10;
        private byte[] _input = new byte[InputSize * InputSize];

        private readonly List<BindingList<RealtimeGraphItem>> _items = new List<BindingList<RealtimeGraphItem>>();
        DispatcherTimer _timer;
        DateTime _last;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private byte[] GenerateInput(int length)
        {
            var result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (byte)_ran.Next(256);
            }

            return result;
        }

        void MainWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GenerateUiObjects(InputSize);
            _input = GenerateInput(InputSize * InputSize);
            DrawInput(_input);

            for (int i = 0; i < ObservedPixels; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = 0,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _items.Add(new BindingList<RealtimeGraphItem>());

                _contentHolder.Children.Add(graph);
                graph.SeriesSource = _items[i];
            }

            for (int i = 0; i < ObservedPixels; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = 0,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _items.Add(new BindingList<RealtimeGraphItem>());

                _contentHolder1.Children.Add(graph);
                graph.SeriesSource = _items[i];
            }


            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            _timer.Tick += TimerTick;
            _last = DateTime.Now;
            _timer.IsEnabled = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - _last;

            for (int i = 0; i < ObservedPixels; i++)
            {
                int previousTime = _items[i].Count > 0 ? _items[i][_items[i].Count - 1].Time : 0;
                var newItem = new RealtimeGraphItem
                {
                    Time = (int) (previousTime + span.TotalMilliseconds),
                    Value = _input[i] / 256.0 //_ran.NextDouble()
                };

                _items[i].Add(newItem);
            }
            _last = DateTime.Now;
        }

        private void DrawInput(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                ((Rectangle)_canvas.Children[i]).Fill = new SolidColorBrush(Color.FromRgb(input[i], input[i], input[i]));
            }
        }

        private void GenerateUiObjects(int length)
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
                    _canvas.Children.Add(pixel);
                }
            }
        }

        private void GenerateClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _input = GenerateInput(InputSize * InputSize);
            DrawInput(_input);
        }
    }
}
