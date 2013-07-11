using System;

namespace TemporalEncoding
{

    //Midget cells -  1/s(LOW)  20/s(BASE)  200/s(HIGH)
    //Parasol cells - 1.5/s(LOW)  30/s(BASE) 600/s(HIGH)
    public class RetinaConverter
    {
        #region Fields

        private static readonly Random Ran = new Random();

        private const int PhotoreceptorsMatrixSize = 5;
        private const int BipolarRfCenterSize = 1;
        private const int BipolarRfOuterSize = 1;
        private const int BipolarMatrixSize = (PhotoreceptorsMatrixSize - 2 * BipolarRfOuterSize) / BipolarRfCenterSize;
        
        private readonly byte[,] _photoreceptorsVoltage;
        private readonly int[,] _onBipolarsVoltage;
        private readonly int[,] _offBipolarsVoltage;

        #endregion


        #region Instance

        public RetinaConverter()
        {
            _photoreceptorsVoltage = new byte[PhotoreceptorsMatrixSize, PhotoreceptorsMatrixSize];
            _offBipolarsVoltage = new int[BipolarMatrixSize, BipolarMatrixSize];
            _onBipolarsVoltage = new int[BipolarMatrixSize, BipolarMatrixSize];

            GenerateRandomVoltage(_photoreceptorsVoltage);
            CalculateBipolarsRfVoltage(_photoreceptorsVoltage, _offBipolarsVoltage, _onBipolarsVoltage);
        }

        #endregion



        #region Methods

        private void GenerateRandomVoltage(byte[,] photoreceptorsVoltage)
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

        private void CalculateBipolarsRfVoltage(byte[,] photoreceptorsVoltage, int[,] offBipolarsVoltage, int[,] onBipolarsVoltage)
        {
            for (int x = 0; x < BipolarMatrixSize; x++)
            {
                for (int y = 0; y < BipolarMatrixSize; y++)
                {
                    var center = 0.0;
                    var outer = 0.0;
                    const int largeRadius = BipolarRfCenterSize + BipolarRfOuterSize;
                    const int smallRadiusSquare = BipolarRfCenterSize * BipolarRfCenterSize;
                    const int largeRadiusSquare = largeRadius * largeRadius;

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

                    offBipolarsVoltage[x, y] = (int)((outer / outerCount) - (center / centerCount));
                    onBipolarsVoltage[x, y] = (int)((center / centerCount) - (outer / outerCount));
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
