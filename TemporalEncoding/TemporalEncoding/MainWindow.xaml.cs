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
        private const int ObservedSensors = 5;
        private const int ObservedBipolars = 9;

        private byte[] _input = new byte[InputSize * InputSize];
        private double[] _onBipolarsInput = new double[ObservedBipolars];
        private double[] _offBipolarsInput = new double[ObservedBipolars];


        private readonly List<BindingList<RealtimeGraphItem>> _sensors = new List<BindingList<RealtimeGraphItem>>();
        private readonly List<BindingList<RealtimeGraphItem>> _onBipolars = new List<BindingList<RealtimeGraphItem>>();
        private readonly List<BindingList<RealtimeGraphItem>> _offBipolars = new List<BindingList<RealtimeGraphItem>>();
        private readonly List<BindingList<RealtimeGraphItem>> _spikeOnBipolars = new List<BindingList<RealtimeGraphItem>>();
        private readonly List<BindingList<RealtimeGraphItem>> _spikeOffBipolars = new List<BindingList<RealtimeGraphItem>>();

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
                result[i] = 255;// (byte)_ran.Next(256);
            }

            result[0] = 0;
            result[1] = 0;
            result[2] = 0;
            result[3] = 0;
            result[4] = 0;

            result[20] = 0;
            result[21] = 0;
            result[22] = 0;
            result[23] = 0;
            result[24] = 0;

            result[5] = 0;
            result[10] = 0;
            result[15] = 0;


            result[9] = 0;
            result[14] = 0;
            result[19] = 0;

            result[12] = 0;



            return result;
        }

        void MainWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GenerateUiObjects(InputSize);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var pixel = new Rectangle
                    {
                        Width = DisplaySize,
                        Height = DisplaySize,
                        Fill = new SolidColorBrush { Color = Colors.White }
                    };
                    Canvas.SetLeft(pixel, j * DisplaySize);
                    Canvas.SetTop(pixel, i * DisplaySize);
                    _canvas1.Children.Add(pixel);

                    pixel = new Rectangle
                    {
                        Width = DisplaySize,
                        Height = DisplaySize,
                        Fill = new SolidColorBrush { Color = Colors.White }
                    };
                    Canvas.SetLeft(pixel, j * DisplaySize);
                    Canvas.SetTop(pixel, i * DisplaySize);
                    _canvas2.Children.Add(pixel);
                }
            }


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var pixel = new Rectangle
                    {
                        Width = DisplaySize,
                        Height = DisplaySize,
                        Fill = new SolidColorBrush { Color = Colors.White }
                    };
                    Canvas.SetLeft(pixel, j * DisplaySize);
                    Canvas.SetTop(pixel, i * DisplaySize);
                    _pCanvas1.Children.Add(pixel);

                    pixel = new Rectangle
                    {
                        Width = DisplaySize,
                        Height = DisplaySize,
                        Fill = new SolidColorBrush { Color = Colors.White }
                    };
                    Canvas.SetLeft(pixel, j * DisplaySize);
                    Canvas.SetTop(pixel, i * DisplaySize);
                    _pCanvas2.Children.Add(pixel);
                }
            }


            _input = GenerateInput(InputSize * InputSize);

            CalculateReceptiveFields(_input, out _offBipolarsInput, out _onBipolarsInput);

            DrawInput(_input);
            DrawBipolars(_onBipolarsInput, _offBipolarsInput);

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

                _onBipolars.Add(new BindingList<RealtimeGraphItem>());

                _contentHolderOn.Children.Add(graph);
                graph.SeriesSource = _onBipolars[i];
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

                _offBipolars.Add(new BindingList<RealtimeGraphItem>());

                _contentHolderOff.Children.Add(graph);
                graph.SeriesSource = _offBipolars[i];
            }


            for (int i = 0; i < ObservedBipolars; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = -0.3,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _spikeOnBipolars.Add(new BindingList<RealtimeGraphItem>());

                _contentHolderSpikeOn.Children.Add(graph);
                graph.SeriesSource = _spikeOnBipolars[i];
            }

            for (int i = 0; i < ObservedBipolars; i++)
            {
                var graph = new RealtimeGraphControl
                {
                    MaxY = 1,
                    MinY = -0.3,
                    AxisColor = Colors.Gray,
                    SeriesColor = Colors.Black,
                    Height = 50,
                };

                _spikeOffBipolars.Add(new BindingList<RealtimeGraphItem>());

                _contentHolderSpikeOff.Children.Add(graph);
                graph.SeriesSource = _spikeOffBipolars[i];
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            _timer.Tick += TimerTick;
            _last = DateTime.Now;
            _timer.IsEnabled = true;
        }

        private void CalculateReceptiveFields(byte[] input, out double[] offBipolarsInput, out double[] onBipolarsInput)
        {
            offBipolarsInput = new double[ObservedBipolars];
            onBipolarsInput = new double[ObservedBipolars];

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    var index = (i + 1) + (j + 1) * 5;
                    var bipolarIndex = i + j * 3;

                    var center = input[index];
                    var outer = (input[index - 1] + input[index + 1] + input[index - 5] + input[index + 5] + input[index + 5 + 1] + _input[index + 5 - 1] + input[index - 5 + 1] + input[index - 5 - 1]) / 8.0;

                    onBipolarsInput[bipolarIndex] = (center - outer);
                    offBipolarsInput[bipolarIndex] = (outer - center);
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - _last;

            for (int i = 0; i < ObservedSensors; i++)
            {
                int previousTime = _sensors[i].Count > 0 ? _sensors[i][_sensors[i].Count - 1].Time : 0;
                var newItem = new RealtimeGraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                    Value = _input[i] / 256.0
                };

                _sensors[i].Add(newItem);
            }


            for (int i = 0; i < ObservedBipolars; i++)
            {
                int previousTime = _onBipolars[i].Count > 0 ? _onBipolars[i][_onBipolars[i].Count - 1].Time : 0;
                var newItem = new RealtimeGraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                    Value = _onBipolarsInput[i] / 256.0
                };
                _onBipolars[i].Add(newItem);
            }

            for (int i = 0; i < ObservedBipolars; i++)
            {
                int previousTime = _offBipolars[i].Count > 0 ? _offBipolars[i][_offBipolars[i].Count - 1].Time : 0;
                var newItem = new RealtimeGraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                    Value = _offBipolarsInput[i] / 256.0
                };
                _offBipolars[i].Add(newItem);
            }


            for (int i = 0; i < ObservedBipolars; i++)
            {
                int previousTime = _spikeOnBipolars[i].Count > 0 ? _spikeOnBipolars[i][_spikeOnBipolars[i].Count - 1].Time : 0;
                double previousValue = _spikeOnBipolars[i].Count > 0 ? _spikeOnBipolars[i][_spikeOnBipolars[i].Count - 1].Value : 0;

                if (previousValue >= 1)
                {
                    previousValue = -0.2;
                }

                var newValue = 0.0;

                var v = (_onBipolarsInput[i]/256.0);
                if (v < 0)
                {
                    v = v/10;
                }

                if (previousValue < 0)
                {
                    newValue = previousValue + v / 4.0  + 0.1;
                }
                else
                {
                    
                    newValue = previousValue + v / 1.0 + 0.1;
                }

                if (newValue > 1)
                {
                    newValue = 1;
                }

                if (newValue < -0.2)
                {
                    newValue += 0.1;
                }

                var newItem = new RealtimeGraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                    Value = newValue
                };
                _spikeOnBipolars[i].Add(newItem);

                if (newItem.Value > 0.9)
                {
                    ((Rectangle) _pCanvas1.Children[i]).Fill = new SolidColorBrush(Colors.DarkBlue);
                }
                else
                {
                    ((Rectangle)_pCanvas1.Children[i]).Fill = new SolidColorBrush(Colors.White);
                }
            }

            for (int i = 0; i < ObservedBipolars; i++)
            {
                int previousTime = _spikeOffBipolars[i].Count > 0 ? _spikeOffBipolars[i][_spikeOffBipolars[i].Count - 1].Time : 0;
                double previousValue = _spikeOffBipolars[i].Count > 0 ? _spikeOffBipolars[i][_spikeOffBipolars[i].Count - 1].Value : 0;

                if (previousValue >= 1)
                {
                    previousValue = -0.2;
                }

                var newValue = 0.0;

                var v = (_offBipolarsInput[i] / 256.0);
                if (v < 0)
                {
                    v = v / 10;
                }

                if (previousValue < 0)
                {
                    newValue = previousValue + v / 8.0 + 0.1;
                }
                else
                {

                    newValue = previousValue + v / 1.0 + 0.1;
                }

                if (newValue > 1)
                {
                    newValue = 1;
                }

                if (newValue < -0.2)
                {
                    newValue += 0.1;
                }

                var newItem = new RealtimeGraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds) + 350,
                    Value = newValue
                };
                _spikeOffBipolars[i].Add(newItem);

                if (newItem.Value > 0.9)
                {
                    ((Rectangle)_pCanvas2.Children[i]).Fill = new SolidColorBrush(Colors.DarkBlue);
                }
                else
                {
                    ((Rectangle)_pCanvas2.Children[i]).Fill = new SolidColorBrush(Colors.White);
                }
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
            CalculateReceptiveFields(_input, out _offBipolarsInput, out _onBipolarsInput);
            DrawInput(_input);
            DrawBipolars(_onBipolarsInput, _offBipolarsInput);
        }

        private void DrawBipolars(double[] onBipolarsInput, double[] offBipolarsInput)
        {


            for (int i = 0; i < onBipolarsInput.Length; i++)
            {
                var color = (byte)((onBipolarsInput[i] + 255) / 2);
                ((Rectangle)_canvas1.Children[i]).Fill = new SolidColorBrush(Color.FromRgb(color, color, color));
            }

            for (int i = 0; i < offBipolarsInput.Length; i++)
            {
                var color = (byte)((offBipolarsInput[i] + 255) / 2);
                ((Rectangle)_canvas2.Children[i]).Fill = new SolidColorBrush(Color.FromRgb(color, color, color));
            }
        }
    }
}
