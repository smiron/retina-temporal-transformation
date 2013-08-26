using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsRetina
{
    public partial class SpikeViewer : Form
    {
        public SpikeViewer()
        {
            InitializeComponent();
            Load += SpikeViewerLoad;
        }

        void SpikeViewerLoad(object sender, EventArgs e)
        {
            var convertor = new SpikeConverter();

            var input = new byte[,] {  {16,  26,  100, 20 },
                                       {64,  200, 64, 36  }, 
                                       {128, 64,  14, 255 },
                                       {31, 137,  12, 165 } };

            convertor.SetInput(input);

            var bmp = new Bitmap(1200, 600);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    gfx.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                }


                var paddingX = 0;
                var paddingY = 0;
                const int size = 7;
                const int maxSize = 350;


                for (int k = 0; k < maxSize; k++)
                {
                    var result = convertor.IterateResult();

                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        for (int j = 0; j < result.GetLength(1); j++)
                        {
                            if (result[i, j])
                            {
                                gfx.FillRectangle(Brushes.Blue, i * size + paddingX, j * size + paddingY, size, size);
                            }
                            else
                            {
                                gfx.FillRectangle(Brushes.Black, i * size + paddingX, j * size + paddingY, size, size);
                            }
                        }
                    }

                    paddingX = ((size * (result.GetLength(0) + 1)) * (k % 32));
                    paddingY = ((size * (result.GetLength(1) + 1)) * (k / 32));

                }
            }






            img.Image = bmp;





        }
    }
}
