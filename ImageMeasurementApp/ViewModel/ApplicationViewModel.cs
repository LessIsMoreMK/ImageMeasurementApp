using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Diagnostics;

namespace ImageMeasurementApp
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members

        private ImageSource mDisplayImage;

        private int mSelectedUnitIndex;
        private double mZoomFactor;

        /// <summary>
        /// Camera pixel size equivalent in Mikrometer (μm)
        /// </summary>
        private double CameraPixelSize = 12.0;

        public double ZoomFactor
        {
            get => mZoomFactor;
            set
            {
                mZoomFactor = value;
                PictureSizeChanged();
            }
        }

        public double ImagePixelWidth { get; set; }
        public double ImagePixelHeight { get; set; }

        public double ImageDisplayWidth { get; set; }
        public double ImageDisplayHeight { get; set; }

        #endregion

        #region Public Properties

        public ApplicationPage CurrentPage { get; private set; } = ApplicationPage.MainPage;

        public BaseViewModel CurrentPageViewModel { get; set; }
        public ImageSource DisplayImage
        {
            get => mDisplayImage;
            set { mDisplayImage = value; }
        }

        public ObservableCollection<string> MeasureUnits { get; }
            = new ObservableCollection<string>
            {
                "Pixels (px)",
                "Centimeter (cm)",
                "Milimeter (mm)",
                "Mikrometer (μm)",
                "Nanometer (nm)"
            };

        public int SelectedUnitIndex
        { 
            get => mSelectedUnitIndex;
            set
            {
                mSelectedUnitIndex = value;
                PictureSizeChanged();
            }
        }

        public void PictureSizeChanged()
        {
            switch(SelectedUnitIndex)
            {
                //Pixel (px)
                case 0:
                    ImageDisplayWidth = ImagePixelWidth / mZoomFactor;
                    ImageDisplayHeight = ImagePixelHeight / mZoomFactor;
                    break;
                //Centimeter (cm)
                case 1:
                    ImageDisplayWidth = ImagePixelWidth / mZoomFactor * CameraPixelSize / 1000 / 10;
                    ImageDisplayHeight = ImagePixelHeight / mZoomFactor * CameraPixelSize / 1000 / 10;
                    break;
                //Milimeter (mm)
                case 2:
                    ImageDisplayWidth = ImagePixelWidth / mZoomFactor * CameraPixelSize / 1000;
                    ImageDisplayHeight = ImagePixelHeight / mZoomFactor * CameraPixelSize / 1000;
                    break;
                //Mikrometer (μm)
                case 3:
                    ImageDisplayWidth = ImagePixelWidth / mZoomFactor * CameraPixelSize;
                    ImageDisplayHeight = ImagePixelHeight / mZoomFactor * CameraPixelSize;
                    break;
                //Nanometer (nm)
                case 4:
                    ImageDisplayWidth = ImagePixelWidth / mZoomFactor * CameraPixelSize * 1000;
                    ImageDisplayHeight = ImagePixelHeight / mZoomFactor * CameraPixelSize * 1000; 
                    break;
            }
        }

        public string RulerTextColor { get; set; } = "Yellow";
        public string RulerGridColor { get; set; } = "Blue";
        public string RulerHorizontalHeight { get; set; } = "25";

        public bool PointerCoordinates { get; set; }
        public bool HorizontalGrid { get; set; }
        public bool VerticalGrid { get; set; }
        

        

        #endregion

        #region Public Commands
        public ICommand LoadImageCommand { get; set; }
        public ICommand LoadDefaultImageCommand { get; set; }
        public ICommand ConvertImageCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor
        /// </summary>
        public ApplicationViewModel()
        {
            LoadDefaultImageCommand = new RelayCommand(LoadDefaultImage);
            LoadImageCommand = new RelayCommand(LoadImage);
            ConvertImageCommand = new RelayCommand(ConvertImage);

        }

        #endregion

        #region Command Methods

        private void LoadImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = (".bmp");
            open.Filter = "Pictures (*.jpg;*.gif;*.png;*.bmp)|*.jpg;*.gif;*.png;*.bmp";

            if (open.ShowDialog() == true)
            {
                var path = open.FileName;
                var byteArr = ImageLocationToByte(path);
                var image = ByteToImage(byteArr);

                DisplayImage = image;
            }
        }

        private void LoadDefaultImage()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "../../../Images/00_Source.bmp");
            var byteArr = ImageLocationToByte(path);
            var image = ByteToImage(byteArr);

            DisplayImage = image;
        }

        #endregion

        #region Public Helper Methods

        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            CurrentPageViewModel = viewModel;

            var different = CurrentPage != page;

            CurrentPage = page;

            if (!different)
                OnPropertyChanged(nameof(CurrentPage));
        }

        #region Image Conversion Methods

        public void ConvertImage()
        {
            //var watch = Stopwatch.StartNew();
            //var temp = ImageLocationToByte(DisplayImage);
            //watch.Stop();
            //var time = watch.ElapsedMilliseconds;

            //var watch2 = Stopwatch.StartNew();
            //ImageResult = ByteToImage(temppp);
            //watch.Stop();-- 
            //var time2 = watch.ElapsedMilliseconds;
            if (DisplayImage == null)
                return;

             var bitmapSource = (BitmapSource)(DisplayImage);
            var bm = BitmapFromSource(bitmapSource);
            var temp = ColorToGrayscale(bm);
            var bit = BitmapToImageSource(temp);

            DisplayImage = bit;
        }

        public Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public byte[] ImageLocationToByte(string imageLocation)
        {
            var img = System.Drawing.Image.FromFile(imageLocation);

            ImagePixelHeight = ImageDisplayHeight = img.Height;
            ImagePixelWidth = ImageDisplayWidth = img.Width;

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

        public static Bitmap ColorToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width,
                h = bmp.Height,
                r, ic, oc, bmpStride, outputStride, bytesPerPixel;
            System.Drawing.Imaging.PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                System.Drawing.Color tmp = System.Drawing.Color.FromArgb(255, i, i, i);
                palette.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)bmp.Clone();

                //Make sure the palette is a grayscale palette and not some other
                //8-bit indexed palette
                output.Palette = palette;

                return output;
            }

            //Get the number of bytes per pixel
            switch (pfIn)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException("Image format not supported");
            }

            //Lock the images
            bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
                                   pfIn);
            outputData = output.LockBits(new System.Drawing.Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
                                         System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                if (bytesPerPixel == 3)
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = .299*R + .587*G + .114*B
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 3, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                (0.299f * bmpPtr[r * bmpStride + ic] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 2]);
                }
                else //bytesPerPixel == 4
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = alpha * (.299*R + .587*G + .114*B)
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 4, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                ((bmpPtr[r * bmpStride + ic] / 255.0f) *
                                (0.299f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 2] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 3]));
                }
            }

            //Unlock the images
            bmp.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
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

        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        #endregion

        #endregion
    }
}