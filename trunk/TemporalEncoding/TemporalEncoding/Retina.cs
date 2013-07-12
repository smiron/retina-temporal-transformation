using System;
using System.Drawing;

namespace TemporalEncoding
{

    //Midget cells -  1/s(LOW)  20/s(BASE)  200/s(HIGH)
    //Parasol cells - 1.5/s(LOW)  30/s(BASE) 600/s(HIGH)
    public class Retina
    {
        #region Fields

        private const int MinFrequency = 1;
        private const int BaseFrequency = 20;
        private const int MaxFrequency = 200;

        private static readonly Random Ran = new Random();

        private readonly int _photoreceptorsMatrixSize;
        private readonly int _bipolarRfCenterSize;
        private readonly int _bipolarRfOuterSize;
        private readonly int _bipolarMatrixSize;

        private readonly byte[,] _photoreceptorsVoltage;
        private readonly int[,] _onBipolarsVoltage;
        private readonly int[,] _offBipolarsVoltage;

        private readonly int[,] _onMigetGanglionFrequency;
        private readonly int[,] _offMigetGanglionFrequency;

        private readonly int[,] _onMigetGanglionVoltage;
        private readonly int[,] _offMigetGanglionVoltage;

        private readonly bool[,] _onMigetGanglionActionPotentials;
        private readonly bool[,] _offMigetGanglionActionPotentials;


        #endregion

        #region Properties

        public int[,] OnBipolarsVoltage
        {
            get { return _onBipolarsVoltage; }
        }

        public int[,] OffBipolarsVoltage
        {
            get { return _offBipolarsVoltage; }
        }

        public byte[,] PhotoreceptorsVoltage
        {
            get { return _photoreceptorsVoltage; }
        }

        public int[,] OnMigetGanglionFrequency
        {
            get { return _onMigetGanglionFrequency; }
        }

        public int[,] OffMigetGanglionFrequency
        {
            get { return _offMigetGanglionFrequency; }
        }

        public int[,] OnMigetGanglionVoltage
        {
            get { return _onMigetGanglionVoltage; }
        }

        public int[,] OffMigetGanglionVoltage
        {
            get { return _offMigetGanglionVoltage; }
        }

        public bool[,] OnMigetGanglionActionPotentials
        {
            get { return _onMigetGanglionActionPotentials; }
        }

        public bool[,] OffMigetGanglionActionPotentials
        {
            get { return _offMigetGanglionActionPotentials; }
        }

        #endregion

        #region Instance

        public Retina(int photoreceptorsMatrixSize = 5, int bipolarRfCenterSize = 1, int bipolarRfOuterSize = 1)
        {
            _bipolarRfCenterSize = bipolarRfCenterSize;
            _bipolarRfOuterSize = bipolarRfOuterSize;

            _photoreceptorsMatrixSize = photoreceptorsMatrixSize;
            _bipolarMatrixSize = (_photoreceptorsMatrixSize - 2 * _bipolarRfOuterSize) / _bipolarRfCenterSize;

            _photoreceptorsVoltage = new byte[_photoreceptorsMatrixSize, _photoreceptorsMatrixSize];
            _offBipolarsVoltage = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _onBipolarsVoltage = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _onMigetGanglionFrequency = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _offMigetGanglionFrequency = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _onMigetGanglionVoltage = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _offMigetGanglionVoltage = new int[_bipolarMatrixSize, _bipolarMatrixSize];
            _onMigetGanglionActionPotentials = new bool[_bipolarMatrixSize, _bipolarMatrixSize];
            _offMigetGanglionActionPotentials = new bool[_bipolarMatrixSize, _bipolarMatrixSize];

            GenerateInitialVoltage(_photoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
            CalculateGanglionFrequency(_offBipolarsVoltage, OffMigetGanglionFrequency);
            CalculateGanglionFrequency(_onBipolarsVoltage, OnMigetGanglionFrequency);
        }

        #endregion

        #region Methods

        private void GenerateInitialVoltage(byte[,] photoreceptorsVoltage)
        {
            for (int i = 0; i < photoreceptorsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < photoreceptorsVoltage.GetLength(1); j++)
                {
                    photoreceptorsVoltage[i, j] = 255;
                }
                photoreceptorsVoltage[2, 2] = 0;
            }
        }


        private void GenerateRandomVoltage(byte[,] photoreceptorsVoltage)
        {
            for (int i = 0; i < photoreceptorsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < photoreceptorsVoltage.GetLength(1); j++)
                {
                    photoreceptorsVoltage[i, j] = (byte)Ran.Next(256);
                }
            }
        }

        private void LoadVoltageFromImage(Bitmap img, byte[,] photoreceptorsVoltage)
        {
            var startX = Ran.Next(img.Width - _photoreceptorsMatrixSize);
            var startY = Ran.Next(img.Height - _photoreceptorsMatrixSize);

            for (int i = 0; i < _photoreceptorsMatrixSize; i++)
            {
                for (int j = 0; j < _photoreceptorsMatrixSize; j++)
                {
                    Color pixel = img.GetPixel(i + startX, j + startY);
                    var grayScale = (byte)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                    photoreceptorsVoltage[i, j] = grayScale;
                }
            }
        }

        public void ChangeRetinaImage(string imgFile)
        {
            var img = new Bitmap(imgFile);
            LoadVoltageFromImage(img, PhotoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
            CalculateGanglionFrequency(_offBipolarsVoltage, OffMigetGanglionFrequency);
            CalculateGanglionFrequency(_onBipolarsVoltage, OnMigetGanglionFrequency);
        }

        public void ChangeRetinaImage()
        {
            GenerateRandomVoltage(_photoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
            CalculateGanglionFrequency(_offBipolarsVoltage, OffMigetGanglionFrequency);
            CalculateGanglionFrequency(_onBipolarsVoltage, OnMigetGanglionFrequency);
        }

        private void CalculateBipolarsRfVoltage(byte[,] photoreceptorsVoltage, int[,] offBipolarsVoltage, int[,] onBipolarsVoltage)
        {
            for (int x = 0; x < _bipolarMatrixSize; x++)
            {
                for (int y = 0; y < _bipolarMatrixSize; y++)
                {
                    var center = 0.0;
                    var outer = 0.0;
                    int largeRadius = _bipolarRfCenterSize + _bipolarRfOuterSize;
                    int smallRadiusSquare = _bipolarRfCenterSize * _bipolarRfCenterSize;
                    int largeRadiusSquare = largeRadius * largeRadius;

                    var centerCount = 0;
                    var outerCount = 0;

                    for (int i = -largeRadius; i < largeRadius; i++)
                    {
                        for (int j = -largeRadius; j < largeRadius; j++)
                        {
                            int r = (i * i + j * j);
                            if (r < (smallRadiusSquare))
                            {
                                center = (center + photoreceptorsVoltage[j + y + _bipolarRfOuterSize, i + x + _bipolarRfOuterSize]);
                                centerCount++;
                            }
                            else if (r < (largeRadiusSquare))
                            {
                                outer = (outer + photoreceptorsVoltage[j + y + _bipolarRfOuterSize, i + x + _bipolarRfOuterSize]);
                                outerCount++;
                            }
                        }
                    }

                    offBipolarsVoltage[y, x] = (int)((outer / outerCount) - (center / centerCount));
                    onBipolarsVoltage[y, x] = (int)((center / centerCount) - (outer / outerCount));
                }
            }
        }

        private void CalculateGanglionFrequency(int[,] bipolarsVoltage, int[,] ganglionFrequency)
        {
            const double plusInterval = (MaxFrequency - BaseFrequency) / 256.0;
            const double minusInterval = (BaseFrequency - MinFrequency) / 256.0;

            for (int i = 0; i < bipolarsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < bipolarsVoltage.GetLength(1); j++)
                {
                    if (bipolarsVoltage[i, j] == 0)
                    {
                        ganglionFrequency[i, j] = 1000 / BaseFrequency;
                    }
                    else if (bipolarsVoltage[i, j] > 0)
                    {
                        ganglionFrequency[i, j] = 1000 / (int) Math.Round((bipolarsVoltage[i, j] * plusInterval));
                    }
                    else
                    {
                        ganglionFrequency[i, j] = 1000 / (int)(20 - (Math.Abs(bipolarsVoltage[i, j]) * minusInterval));
                    }
                }
            }
        }

        public void IterateTime()
        {
            
            for (int i = 0; i < OnMigetGanglionVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < OnMigetGanglionVoltage.GetLength(1); j++)
                {
                    OnMigetGanglionVoltage[i, j]++;
                    OffMigetGanglionVoltage[i, j]++;

                    if (OnMigetGanglionVoltage[i, j] > OnMigetGanglionFrequency[i, j])
                    {
                        OnMigetGanglionVoltage[i, j] = 0;
                        _onMigetGanglionActionPotentials[i, j] = true;
                    }
                    else
                    {
                        _onMigetGanglionActionPotentials[i, j] = false;
                    }

                    if (OffMigetGanglionVoltage[i, j] > OffMigetGanglionFrequency[i, j])
                    {
                        OffMigetGanglionVoltage[i, j] = 0;
                        _offMigetGanglionActionPotentials[i, j] = true;
                    }
                    else
                    {
                        _offMigetGanglionActionPotentials[i, j] = false;
                    }
                }
            }

           
        }

        #endregion

    }
}
