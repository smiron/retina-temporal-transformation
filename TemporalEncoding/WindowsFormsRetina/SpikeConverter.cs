using System;

namespace WindowsFormsRetina
{
    public class SpikeConverter
    {
        #region Fields

        private static readonly Random Ran = new Random();
        private const int SpikeMaxFrequency = 256;

        private int _imgWidth;
        private int _imgHeight;

        private int[,] _spikeFrequency;
        private int[,] _voltage;

        #endregion

        #region Methods

        private void Init(int width, int height)
        {
            _imgWidth = width;
            _imgHeight = height;

            _spikeFrequency = new int[_imgWidth, _imgHeight];
            _voltage = new int[_imgWidth, _imgHeight];

            for (int i = 0; i < _imgWidth; i++)
            {
                for (int j = 0; j < _imgHeight; j++)
                {
                    _voltage[i, j] = Ran.Next(SpikeMaxFrequency);
                }
            }
        }
        
        public void SetInput(byte[,] input)
        {
            if (input.GetLength(0) != _imgWidth || input.GetLength(1) != _imgHeight)
            {
                Init(input.GetLength(0), input.GetLength(1));
            }

            for (int i = 0; i < _imgWidth; i++)
            {
                for (int j = 0; j < _imgHeight; j++)
                {
                    _spikeFrequency[i, j] = input[i, j] % SpikeMaxFrequency;
                }
            }
        }

        public bool[,] IterateResult()
        {
            var result = new bool[_imgWidth, _imgWidth];

            for (int i = 0; i < _imgWidth; i++)
            {
                for (int j = 0; j < _imgHeight; j++)
                {
                    _voltage[i, j] += _spikeFrequency[i, j];

                    if (_voltage[i, j] >= SpikeMaxFrequency)
                    {
                        result[i, j] = true;
                        _voltage[i, j] -= SpikeMaxFrequency;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
