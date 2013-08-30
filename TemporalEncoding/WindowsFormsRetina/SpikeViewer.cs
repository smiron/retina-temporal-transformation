using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsRetina.Htm;

namespace WindowsFormsRetina
{
    public partial class SpikeViewer : Form
    {
        private const string ImgPath = @"C:\temp\temporal\TemporalEncoding\TemporalEncoding\Images\Retina.jpg";
        private readonly Random _ran = new Random();
        private const int InputSize = 8;
        private const int ColumnSize = 4;

        private readonly SpikeConverter _convertor = new SpikeConverter();
        private HtmSpatialPooler _spatialPooler = new HtmSpatialPooler(InputSize, InputSize, ColumnSize, ColumnSize);


        public SpikeViewer()
        {
            InitializeComponent();
            Load += SpikeViewerLoad;

            _xx = _ran.Next(20)+200;
            _yy = _ran.Next(20)+300;
        }


        private int _xx;
        private int _yy;

        private byte[,] GetInput()
        {


            var result = new byte[InputSize, InputSize];


            var src = Image.FromFile(ImgPath);

            if (_ran.Next() % 2 == 0)
            {
                _xx += _ran.Next(40);
            }
            else
            {
                _xx -= _ran.Next(40);
            }

            if (_ran.Next() % 2 == 0)
            {
                _yy += _ran.Next(40);
            }
            else
            {
                _yy -= _ran.Next(40);
            }
            

            if (_xx <= 0 || _yy <= 0 || _xx >= (src.Width - InputSize) || _yy >= (src.Height - InputSize))
            {
                _xx = _ran.Next(src.Width - InputSize);
                _yy = _ran.Next(src.Height - InputSize);
            }
            

            var cropRect = new Rectangle(_xx, _yy, InputSize, InputSize);

            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
            }

            for (int i = 0; i < target.Width; i++)
            {
                for (int j = 0; j < target.Height; j++)
                {
                    result[i, j] = target.GetPixel(i, j).R;
                }
            }

            source.Image = target;

            return result;
        }

        private void DoLoadSpikes()
        {
            var bmp = new Bitmap(1200, 600);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    gfx.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                }


                var paddingX = 0;
                var paddingY = 0;
                const int size = 8;
                const int columnSize = 8;
                const int maxSize = 256;

                

                for (int i = 0; i < 1; i++)
                {

                    var input = GetInput();
                    _convertor.SetInput(input);


                    for (int k = 0; k < maxSize; k++)
                    {
                        var result = _convertor.IterateResult();

                        _spatialPooler.SetInput(result);
                        _spatialPooler.Run();

                        //for (int i = 0; i < result.GetLength(0); i++)
                        //{
                        //    for (int j = 0; j < result.GetLength(1); j++)
                        //    {
                        //        if (result[i, j])
                        //        {
                        //            gfx.FillRectangle(Brushes.Blue, i * size + paddingX, j * size + paddingY, size, size);
                        //        }
                        //        else
                        //        {
                        //            gfx.FillRectangle(Brushes.Black, i * size + paddingX, j * size + paddingY, size, size);
                        //        }

                        //        if (i < ColumnSize && j < ColumnSize)
                        //        {
                        //            gfx.FillRectangle(Brushes.Black, i * columnSize + paddingX, j * columnSize + paddingY + size * (result.GetLength(0) + 1), columnSize, columnSize);
                        //        }

                        //    }
                        //}


                        //foreach (var activeColumn in _spatialPooler.ActiveColumns)
                        //{
                        //    gfx.FillRectangle(Brushes.Red, activeColumn.X * columnSize + paddingX, activeColumn.Y * columnSize + paddingY + size * (result.GetLength(0) + 1), columnSize, columnSize);
                        //}

                        //paddingX = ((size * (result.GetLength(0) + 1)) * (k % 32));
                        //paddingY = ((size * (result.GetLength(1) + 1) * 2) * (k / 32));
                    }
                }

                foreach (var column in _spatialPooler.Columns)
                {
                    paddingX = column.X * size * 10;
                    paddingY = column.Y * size * 10;

                    foreach (var synapse in column.PotentialSynapses)
                    {
                        gfx.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, (byte)(Math.Min(synapse.Permanance * column.Boost * 255,255)))), synapse.X * size + paddingX, synapse.Y * size + paddingY, size, size);
                    }

                   


                }
            }


            output.Image = bmp;
        }

        void SpikeViewerLoad(object sender, EventArgs e)
        {
            //DoLoadSpikes();
        }

        private void LoadSpikes(object sender, EventArgs e)
        {

            while (true)
            {
                DoLoadSpikes();
                Application.DoEvents();
            }
            
        }
    }
}
