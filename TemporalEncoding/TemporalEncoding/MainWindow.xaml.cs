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
        private const int ObservedSensors = 25;
        private const int ObservedBipolars = 9;

        private byte[] _input = new byte[InputSize * InputSize];

        private readonly List<BindingList<RealtimeGraphItem>> _sensors = new List<BindingList<RealtimeGraphItem>>();
        private readonly List<BindingList<RealtimeGraphItem>> _bipolars = new List<BindingList<RealtimeGraphItem>>();

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

            for (int i = 0; i < ObservedSensors; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = 0,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _sensors.Add(new BindingList<RealtimeGraphItem>());

                _contentHolder.Children.Add(graph);
                graph.SeriesSource = _sensors[i];
            }

            for (int i = 0; i < ObservedBipolars; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = -1,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _bipolars.Add(new BindingList<RealtimeGraphItem>());

                _contentHolder1.Children.Add(graph);
                graph.SeriesSource = _bipolars[i];
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

            for (int i = 0; i < ObservedSensors; i++)
            {
                int previousTime = _sensors[i].Count > 0 ? _sensors[i][_sensors[i].Count - 1].Time : 0;
                var newItem = new RealtimeGraphItem
                {
                    Time = (int) (previousTime + span.TotalMilliseconds)+350,
                    Value = _input[i] / 256.0 //_ran.NextDouble()
                };

                _sensors[i].Add(newItem);
            }


            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++) 
                {
                    
                    var index = (i + 1) + (j + 1)*5;
                    var bipolarIndex = i + j * 3;

                    int previousTime = _bipolars[bipolarIndex].Count > 0 ? _bipolars[bipolarIndex][_bipolars[bipolarIndex].Count - 1].Time : 0;
                    var newItem = new RealtimeGraphItem
                    {
                        Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                        Value = (_input[index] -(_input[index - 1] + _input[index + 1] + _input[index - 3] + _input[index + 3] + _input[index + 3 + 1] + _input[index + 3 - 1] + +_input[index - 3 + 1] + _input[index - 3 - 1]) / 8.0) / 256.0 //_ran.NextDouble()
                    };
                    _bipolars[bipolarIndex].Add(newItem);

                }
            }
            //{
            //    int previousTime = _bipolars[i].Count > 0 ? _bipolars[i][_bipolars[i].Count - 1].Time : 0;
            //    var newItem = new RealtimeGraphItem
            //    {
            //        Time = (int)(previousTime + span.TotalMilliseconds) + 350,
            //        Value = _input[i] / 256.0 //_ran.NextDouble()
            //    };

            //    _bipolars[i].Add(newItem);
            //}

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
