
namespace ImageMeasurementApp
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : BasePage<ApplicationViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(ApplicationViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
