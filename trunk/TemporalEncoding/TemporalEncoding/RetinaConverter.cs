using System;
using System.Drawing;

namespace TemporalEncoding
{

    //Midget cells -  1/s(LOW)  20/s(BASE)  200/s(HIGH)
    //Parasol cells - 1.5/s(LOW)  30/s(BASE) 600/s(HIGH)
    public class RetinaConverter
    {
        #region Fields

        private static readonly Random Ran = new Random();

        private int PhotoreceptorsMatrixSize;
        private int BipolarRfCenterSize;
        private int BipolarRfOuterSize;
        private int BipolarMatrixSize;

        private readonly byte[,] _photoreceptorsVoltage;
        private readonly int[,] _onBipolarsVoltage;
        private readonly int[,] _offBipolarsVoltage;

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

        #endregion

        #region Instance

        public RetinaConverter(int photoreceptorsMatrixSize = 5, int bipolarRfCenterSize = 1, int bipolarRfOuterSize = 1)
        {
            BipolarRfCenterSize = bipolarRfCenterSize;
            BipolarRfOuterSize = bipolarRfOuterSize;

            PhotoreceptorsMatrixSize = photoreceptorsMatrixSize;
            BipolarMatrixSize = (PhotoreceptorsMatrixSize - 2 * BipolarRfOuterSize) / BipolarRfCenterSize;

            _photoreceptorsVoltage = new byte[PhotoreceptorsMatrixSize, PhotoreceptorsMatrixSize];
            _offBipolarsVoltage = new int[BipolarMatrixSize, BipolarMatrixSize];
            _onBipolarsVoltage = new int[BipolarMatrixSize, BipolarMatrixSize];

            GenerateInitialVoltage(_photoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
        }

        #endregion

        #region Methods

        private void GenerateInitialVoltage(byte[,] photoreceptorsVoltage)
        {
            for (int i = 0; i < photoreceptorsVoltage.GetLength(0); i++)
            {
                for (int j = 0; j < photoreceptorsVoltage.GetLength(1); j++)
                {
                    photoreceptorsVoltage[i, j] = 255; // (byte)Ran.Next(256);
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
            var startX = Ran.Next(img.Width - PhotoreceptorsMatrixSize);
            var startY = Ran.Next(img.Height - PhotoreceptorsMatrixSize);

            for (int i = 0; i < PhotoreceptorsMatrixSize; i++)
            {
                for (int j = 0; j < PhotoreceptorsMatrixSize; j++)
                {
                    Color pixel = img.GetPixel(i + startX, j + startY);
                    byte grayScale = (byte)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
                    photoreceptorsVoltage[i, j] = grayScale;
                }
            }
        }

        public void NewFrame(string imgFile)
        {
            Bitmap img = new Bitmap(imgFile);
            LoadVoltageFromImage(img, PhotoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
        }

        public void NewFrame()
        {
            GenerateRandomVoltage(_photoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
        }

        private void CalculateBipolarsRfVoltage(byte[,] photoreceptorsVoltage, int[,] offBipolarsVoltage, int[,] onBipolarsVoltage)
        {
            for (int x = 0; x < BipolarMatrixSize; x++)
            {
                for (int y = 0; y < BipolarMatrixSize; y++)
                {
                    var center = 0.0;
                    var outer = 0.0;
                    int largeRadius = BipolarRfCenterSize + BipolarRfOuterSize;
                    int smallRadiusSquare = BipolarRfCenterSize * BipolarRfCenterSize;
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
                                center = (center + photoreceptorsVoltage[j + y + BipolarRfOuterSize, i + x + BipolarRfOuterSize]);
                                centerCount++;
                            }
                            else if (r < (largeRadiusSquare))
                            {
                                outer = (outer + photoreceptorsVoltage[j + y + BipolarRfOuterSize, i + x + BipolarRfOuterSize]);
                                outerCount++;
                            }
                        }
                    }

                    offBipolarsVoltage[y,x] = (int)((outer / outerCount) - (center / centerCount));
                    onBipolarsVoltage[y,x] = (int)((center / centerCount) - (outer / outerCount));
                }
            }
        }

        public byte[,] GetSpikeMatrix()
        {



            return null;
        }

        #endregion

    }
}
