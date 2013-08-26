using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsRetina
{
    public partial class SpikeViewer : Form
    {
        private const string ImgPath = @"C:\temp\temporal\TemporalEncoding\TemporalEncoding\Images\TestImage.png";
        private readonly Random _ran = new Random();
        private readonly SpikeConverter _convertor = new SpikeConverter();

        public SpikeViewer()
        {
            InitializeComponent();
            Load += SpikeViewerLoad;
        }



        private byte[,] GetInput()
        {
            const int size = 16;

            var result = new byte[size,size];


            var src = Image.FromFile(ImgPath);
            
            var startX = _ran.Next(src.Width - size);
            var startY = _ran.Next(src.Height - size);

            var cropRect = new Rectangle(startX, startY, size, size);
            
            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),cropRect,GraphicsUnit.Pixel);
            }

            for (int i = 0; i < target.Width; i++)
            {
                for (int j = 0; j < target.Height; j++)
                {
                    result[i, j] = target.GetPixel(i, j).R;
                }
            }

            return result;
        }


        void SpikeViewerLoad(object sender, EventArgs e)
        {
            DoLoadSpikes();
        }

        private void DoLoadSpikes()
        {
            var input = GetInput();

            _convertor.SetInput(input);

            var bmp = new Bitmap(1200, 600);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    gfx.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                }


                var paddingX = 0;
                var paddingY = 0;
                const int size = 2;
                const int maxSize = 350;


                for (int k = 0; k < maxSize; k++)
                {
                    var result = _convertor.IterateResult();

                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        for (int j = 0; j < result.GetLength(1); j++)
                        {
                            if (result[i, j])
                            {
                                gfx.FillRectangle(Brushes.Blue, i*size + paddingX, j*size + paddingY, size, size);
                            }
                            else
                            {
                                gfx.FillRectangle(Brushes.Black, i*size + paddingX, j*size + paddingY, size, size);
                            }
                        }
                    }

                    paddingX = ((size*(result.GetLength(0) + 1))*(k%32));
                    paddingY = ((size*(result.GetLength(1) + 1))*(k/32));
                }
            }


            img.Image = bmp;
        }

        private void LoadSpikes(object sender, EventArgs e)
        {
            DoLoadSpikes();
        }
    }
}
