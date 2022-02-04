using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageMeasurementApp
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Members

        private ImageSource mDisplayImage;

        #endregion

        #region Public Properties

        public ImageSource DisplayImage
        {
            get => mDisplayImage;
            set { mDisplayImage = value; }
        }

        public bool PointerCoordinates { get; set; }
        public bool HorizontalGrid { get; set; }
        public bool VerticalGrid { get; set; }

        #endregion

        #region Commands

        public ICommand LoadImageCommand { get; set; }
        public ICommand LoadDefaultImageCommand { get; set; }
        public ICommand ConvertImageCommand { get; set; }

        public ObservableCollection<string> MeasureUnits { get; set; }
        public string SelectedUnits { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            MeasureUnits = new ObservableCollection<string>();
            MeasureUnits.Add("Centimeter (cm)");
            MeasureUnits.Add("Milimeter (mm)");
            MeasureUnits.Add("Mikrometer (μm)");
            MeasureUnits.Add("Nanometer (nm)");


            LoadDefaultImageCommand = new RelayCommand(LoadDefaultImage);
            LoadImageCommand = new RelayCommand(LoadImage);
            ConvertImageCommand = new RelayCommand(ConvertImage);
        }

        #endregion

        #region Methods

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

        public void ConvertImage()
        {
            //var watch = Stopwatch.StartNew();
            //var temp = ImageLocationToByte(DisplayImage);
            //watch.Stop();
            //var time = watch.ElapsedMilliseconds;

            //var watch2 = Stopwatch.StartNew();
            //ImageResult = ByteToImage(temppp);
            //watch.Stop();
            //var time2 = watch.ElapsedMilliseconds;

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
            var img = Image.FromFile(imageLocation);

            ImageHeight = img.Height;
            ImageWidth = img.Width;

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
            PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                System.Drawing.Color tmp = System.Drawing.Color.FromArgb(255, i, i, i);
                palette.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == PixelFormat.Format8bppIndexed)
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
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException("Image format not supported");
            }

            //Lock the images
            bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
                                   pfIn);
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
                                         PixelFormat.Format8bppIndexed);
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
    }
}
