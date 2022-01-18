using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageMeasurementApp
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Members

        private string mImagePath;

        #endregion

        #region Properties

        public string ImagePath
        {
            get => mImagePath;
            set { mImagePath = value; }
        }

        #endregion

        #region Commands

        public ICommand LoadImageCommand { get; set; }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
        }
        #endregion

        #region Methods

        private void LoadImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = (".png");
            open.Filter = "Pictures (*.jpg;*.gif;*.png)|*.jpg;*.gif;*.png";

            if (open.ShowDialog() == true)
                ImagePath = open.FileName;
            var a = 1;
        }

        #endregion
    }
}
