namespace TemporalEncoding
{
    public partial class RetinaGeneratorUi 
    {
        public RetinaGeneratorUi()
        {
            InitializeComponent();
            Loaded += RetinaGeneratorUi_Loaded;
        }

        void RetinaGeneratorUi_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var retina = new RetinaConverter();


            var mat = retina.GetSpikeMatrix();
        }
    }
}
