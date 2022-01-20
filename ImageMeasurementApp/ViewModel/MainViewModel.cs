using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace ImageMeasurementApp
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Members

        private string mImagePath;

        private ImageSource mImageResult;

        #endregion

        #region Properties

        public string ImagePath
        {
            get => mImagePath;
            set { mImagePath = value; }
        }

        public ImageSource ImageResult
        {
            get => mImageResult;
            set { mImageResult = value; }
        }

        #endregion

        #region Commands

        public ICommand LoadImageCommand { get; set; }
        public ICommand LoadDefaultImageCommand { get; set; }
        public ICommand ConvertImageCommand { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            LoadDefaultImageCommand = new RelayCommand(LoadDefaultImage);
            LoadImageCommand = new RelayCommand(LoadImage);
            ConvertImageCommand = new RelayCommand(ConvertImage);
        }

        #endregion

        #region Methods

        public void ConvertImage()
        {
            var watch = Stopwatch.StartNew();
            var temppp = ImageLocationToByte(ImagePath);
            watch.Stop();
            var time = watch.ElapsedMilliseconds;

            var watch2 = Stopwatch.StartNew();
            ImageResult = ByteToImage(temppp);
            watch.Stop();
            var time2 = watch.ElapsedMilliseconds;

        }

        private void LoadImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = (".png");
            open.Filter = "Pictures (*.jpg;*.gif;*.png;*.bmp)|*.jpg;*.gif;*.png;*.bmp";

            if (open.ShowDialog() == true)
                ImagePath = open.FileName;
        }

        private void LoadDefaultImage()
        {
            ImagePath = Path.Combine(Environment.CurrentDirectory, "Images/00_Source.bmp");
        }

        public byte[] ImageLocationToByte(string imageLocation)
{
            var img = Image.FromFile(imageLocation);
            var height = img.Height;
            var width = img.Width;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }

            /*byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            fs.Close();
            return imageData;*/
        }

        public ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        #endregion
    }
}
