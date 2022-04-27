using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Biometria_1
{
    /// <summary>
    /// Logika interakcji dla klasy OldWindow.xaml
    /// </summary>
    public partial class OldWindow : Window
    {

        //public readonly static string ImageFilePath = "../../../A.jpg";
        //public readonly static string ImageFilePath = "../../../text2.png";
        public readonly static string ImageFilePath = "../../../testowy.jpg";
        //public readonly static string ImageFilePath = "../../../Tree.jpg";
        public readonly static string ResultFilePath = "../../../Result.jpg";

        private bool ready = false;

        public Bitmap BitmapSrc;
        public Bitmap BitmapResult;
        public OldWindow()
        {
            BitmapSrc = new Bitmap(ImageFilePath);
            InitializeComponent();

            Threshold.Minimum = byte.MinValue;
            Threshold.Maximum = byte.MaxValue;

            StretchingMin.Minimum = 0;
            StretchingMin.Maximum = 254;
            StretchingMax.Minimum = 1;
            StretchingMax.Maximum = 255;

            UpdateImage();
            ready = true;
        }

        public enum HistogramMode
        {
            Red,
            Green,
            Blue,
            Average
        }

        public Bitmap Histogram(Bitmap bitmap, HistogramMode mode)
        {
            var data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);

            int[] histogramValues = GetHistogram(mode, bitmapData);

            double maxValue = histogramValues.Max();

            for (int i = 0; i < histogramValues.Length; i++)
            {
                histogramValues[i] = (int)(histogramValues[i] / maxValue * data.Height);
            }

            bitmapData = new byte[bitmapData.Length];
            for (int i = 0; i < bitmapData.Length; i++)
            {
                bitmapData[i] = byte.MaxValue;
            }


            for (int i = 0; i < histogramValues.Length; i++)
            {
                for (int j = 0; j < histogramValues[i]; j++)
                {
                    int index = i * 3 + (data.Height - 1 - j) * data.Stride + data.Width / byte.MaxValue * i;

                    bitmapData[index] = bitmapData[index + 1] = bitmapData[index + 2] = 0;
                }
            }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }

        private int[] GetHistogram(HistogramMode mode, byte[] bitmapData)
        {
            int[] histogramValues = new int[byte.MaxValue + 1];
            if (mode == HistogramMode.Average)
            {
                for (int i = 0; i < bitmapData.Length; i += 3)
                {
                    int value = (bitmapData[i] + bitmapData[i + 1] + bitmapData[i + 2]) / 3;
                    histogramValues[value]++;
                }
            }
            else if (mode == HistogramMode.Blue)
            {
                for (int i = 0; i < bitmapData.Length; i += 3)
                {
                    histogramValues[bitmapData[i]]++;
                }
            }
            else if (mode == HistogramMode.Green)
            {
                for (int i = 1; i < bitmapData.Length; i += 3)
                {
                    histogramValues[bitmapData[i]]++;
                }
            }
            else if (mode == HistogramMode.Red)
            {
                for (int i = 2; i < bitmapData.Length; i += 3)
                {
                    histogramValues[bitmapData[i]]++;
                }
            }
            else
            {
                throw new Exception("Wrong histogram mode");
            }
            return histogramValues;
        }



        public Bitmap BinaryThreshold(Bitmap bitmap)
        {
            return BinaryThreshold(bitmap, (byte)Threshold.Value);
        }

        public Bitmap BinaryThreshold(Bitmap bitmap, byte threshold, bool average)
        {
            return BinaryThreshold(bitmap, threshold, false, false, false, average);
        }

        public Bitmap BinaryThreshold(Bitmap bitmap, byte threshold)
        {
            return BinaryThreshold(bitmap,
                threshold,
                RedCheckBox.IsChecked.Value,
                GreenCheckBox.IsChecked.Value,
                BlueCheckBox.IsChecked.Value,
                AverageCheckBox.IsChecked.Value);
        }

        public Bitmap BinaryThreshold(Bitmap bitmap, byte threshold, bool red, bool green, bool blue, bool average)
        {
            var data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);

            for (int i = 0; i < bitmapData.Length; i += 3)
            {
                byte b = blue || average ? bitmapData[i] : byte.MinValue;
                byte g = green || average ? bitmapData[i + 1] : byte.MinValue;
                byte r = red || average ? bitmapData[i + 2] : byte.MinValue;

                byte result = (byte)((r + g + b) / 3);
                if (average)
                {
                    bitmapData[i] = result > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 1] = result > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 2] = result > threshold ? byte.MaxValue : byte.MinValue;
                }
                else
                {
                    bitmapData[i] = b > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 1] = g > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 2] = r > threshold ? byte.MaxValue : byte.MinValue;
                }
            }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }

        public BitmapImage ConvertToImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void CheckHistogram()
        {
            if (!ready) return;
            if (HistogramCheckBox.IsChecked.Value)
            {
                HistogramMode histogram;
                ResultMessage.Content = "Histogram: ";
                if (AverageCheckBox.IsChecked.Value)
                {
                    ResultMessage.Content += "Average";
                    histogram = HistogramMode.Average;
                }
                else if (RedCheckBox.IsChecked.Value)
                {
                    ResultMessage.Content += "Red";
                    histogram = HistogramMode.Red;
                }
                else if (GreenCheckBox.IsChecked.Value)
                {
                    ResultMessage.Content += "Green";
                    histogram = HistogramMode.Green;
                }
                else if (BlueCheckBox.IsChecked.Value)
                {
                    ResultMessage.Content += "Blue";
                    histogram = HistogramMode.Blue;
                }
                else
                {
                    ResultMessage.Content += "Average";
                    histogram = HistogramMode.Average;
                }
                BitmapResult = new Bitmap(BitmapSrc);
                if (StretchingCheckBox.IsChecked.Value)
                    BitmapResult = HistogramStretching(BitmapResult);
                if (EqCheckBox.IsChecked.Value)
                    BitmapResult = HistogramEqual(BitmapResult);
                ImageResult.Source = ConvertToImage(Histogram(BitmapResult, histogram));
            }
            else
            {
                UpdateImage();
            }
        }

        private void UpdateImage()
        {

            BitmapResult = new Bitmap(BitmapSrc);
            if (StretchingCheckBox.IsChecked.Value)
                BitmapResult = HistogramStretching(BitmapResult);
            if (EqCheckBox.IsChecked.Value)
                BitmapResult = HistogramEqual(BitmapResult);
            if (OtsuCheckBox.IsChecked.Value)
                BitmapResult = OtsuMethod(BitmapResult);
            if (NiblackCheckBox.IsChecked.Value)
                BitmapResult = NiBlack(BitmapResult, 7);
            if (BernsenCheckBox.IsChecked.Value)
                BitmapResult = Bernsen(BitmapResult, 7);



            if (int.TryParse(EnterHell.Text, out int hell))
            {
                if (PixelizationCheckBox.IsChecked.Value)
                {
                    BitmapResult = FPixel2(BitmapResult, hell);
                    BitmapResult = OtsuMethod(BitmapResult);
                }
                if (MedianCheckBox.IsChecked.Value)
                    BitmapResult = FMedian(BitmapResult, hell);
                if (KuwaharaCheckBox.IsChecked.Value)
                    BitmapResult = Kuwahara(BitmapResult, hell);
            }

            if (RedCheckBox.IsChecked.Value || GreenCheckBox.IsChecked.Value || BlueCheckBox.IsChecked.Value || AverageCheckBox.IsChecked.Value)
            {
                ImageResult.Source = ConvertToImage(BinaryThreshold(BitmapResult));
                ResultMessage.Content = "Binary image: ";
                if (AverageCheckBox.IsChecked.Value)
                    ResultMessage.Content += "Average ";
                else
                {
                    if (RedCheckBox.IsChecked.Value)
                        ResultMessage.Content += "Red ";
                    if (GreenCheckBox.IsChecked.Value)
                        ResultMessage.Content += "Green ";
                    if (BlueCheckBox.IsChecked.Value)
                        ResultMessage.Content += "Blue ";
                }
                ResultMessage.Content += "Threshold: " + (byte)Threshold.Value;
            }
            else
            {
                //BitmapResult = Kuwahara(BitmapResult);
                ImageResult.Source = ConvertToImage(BitmapResult);
                ResultMessage.Content = "Normal image";
            }
        }

        private byte[] LockBitmap(Bitmap bitmap, ref System.Drawing.Imaging.BitmapData data)
        {
            data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);

            return bitmapData;
        }


        private void UpdateImage(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CheckHistogram();
        }

        private void UpdateImage(object sender, RoutedEventArgs e)
        {
            CheckHistogram();
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            BitmapResult.Save(ResultFilePath);
        }

        private void CheckHistogram(object sender, RoutedEventArgs e)
        {
            CheckHistogram();
        }

        private int[] calculateLUTequal(int[] values, int size)
        {
            double minValue = values.Min();
            int[] output = new int[256];
            double Dn = 0;
            for (int i = 0; i < 256; i++)
            {
                Dn += values[i];
                output[i] = (int)(((Dn - minValue) / (size - minValue)) * 255.0);
            }

            return output;
        }

        private Bitmap HistogramEqual(Bitmap bitmap)
        {
            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    red[color.R]++;
                    green[color.G]++;
                    blue[color.B]++;
                }
            }

            int[] LUTred = calculateLUTequal(red, bitmap.Width * bitmap.Height);
            int[] LUTgreen = calculateLUTequal(green, bitmap.Width * bitmap.Height);
            int[] LUTblue = calculateLUTequal(blue, bitmap.Width * bitmap.Height);
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);


            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(x, y);
                    System.Drawing.Color newpixel = System.Drawing.Color.FromArgb(LUTred[pixel.R], LUTgreen[pixel.G], LUTblue[pixel.B]);
                    newBitmap.SetPixel(x, y, newpixel);
                }
            }
            return newBitmap;
        }
        public Bitmap HistogramEqualization(Bitmap bitmap)
        {
            var data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);
            bitmap.UnlockBits(data);

            double sum = 0;
            var minValue = bitmapData.Min();

            int[] LUT = new int[bitmapData.Length];

            for (int i = 0; i < bitmapData.Length; i++)
            {
                sum += bitmapData[i];
                LUT[i] = (byte)((float)(sum - minValue) / (bitmapData.Length - minValue) * 255);
            }

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(x, y);
                    System.Drawing.Color newpixel = System.Drawing.Color.FromArgb(LUT[pixel.B], LUT[pixel.B], LUT[pixel.B]);
                    newBitmap.SetPixel(x, y, newpixel);
                }
            }

            //Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            //bitmap.UnlockBits(data);

            return newBitmap;
        }

        private Bitmap OtsuMethod(Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap(bitmap, ref data);
            int[] histogramValues = GetHistogram(HistogramMode.Average, bitmapData);

            float weightedSumMax = 0;
            for (int i = 0; i < 256; i++)
            {
                weightedSumMax += i * histogramValues[i];
            }

            int total = data.Height * data.Width;
            int sumBefore = 0;
            int threshold = 0;
            float weightedSumBefore = 0;
            float maxvalue = 0;

            for (int i = 0; i < 256; i++)
            {
                sumBefore += histogramValues[i];
                if (sumBefore <= 0) continue;

                int sumAfter = total - sumBefore;
                if (sumAfter <= 0) break;

                weightedSumBefore += (float)(i * histogramValues[i]);

                float mB = weightedSumBefore / sumBefore;
                float mF = (weightedSumMax - weightedSumBefore) / sumAfter;

                float varBetween = (float)sumBefore * sumAfter * (mB - mF) * (mB - mF);
                if (varBetween > maxvalue)
                {
                    maxvalue = varBetween;
                    threshold = i;
                }
            }
            bitmap.UnlockBits(data);
            return BinaryThreshold(bitmap, (byte)threshold, true);
        }

        public Bitmap HistogramStretching(Bitmap bitmap)
        {
            return HistogramStretching(bitmap, (int)StretchingMin.Value, (int)StretchingMax.Value);
        }

        public Bitmap HistogramStretching(Bitmap bitmap, int minValue, int maxValue)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap(bitmap, ref data);

            for (int i = 0; i < bitmapData.Length; i++)
            {
                int diff = (bitmapData[i] - minValue >= 0) ? (bitmapData[i] - minValue) : 0;
                bitmapData[i] = (byte)((float)diff / (maxValue - minValue) * 255);
            }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }

        public Bitmap NiBlack(Bitmap bitmap, int w = 2, float k = -0.2f)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            //int imgSize = data.Height * data.Width;
            //imgN = copy(img);

            // Calculate the radius of the neighbourhood
            //int w = (n - 1) / 2;

            // Process the image
            for (int i = w + 1; i < dx - w; i++)
            {
                for (int j = w + 1; j < dy - w; j++)
                {
                    List<double> neighbours = new List<double>();
                    // Extract the neighbourhood area
                    for (int x = i - w; x < i + w; x++)
                    {
                        for (int y = j - w; y < j + w; y++)
                        {
                            float bbb = bitmapDataIn[x + y * data.Stride] + bitmapDataIn[x + y * data.Stride + 1] + bitmapDataIn[x + y * data.Stride + 2];
                            bbb /= 3;
                            neighbours.Add(bbb);
                        }
                    }
                    //block = bitmapData[i - w:i + w, j - w:j + w];

                    // Calculate the mean and standard deviation of the neighbourhood region
                    float wBmn = (float)Median(neighbours);
                    float wBstd = (float)standardDeviation(neighbours);

                    // Calculate the threshold value
                    float wBTH = (wBmn + k * wBstd);

                    // Threshold the pixel
                    float aaa = bitmapDataIn[i + j * data.Stride] +
                        bitmapDataIn[i + j * data.Stride + 1] +
                        bitmapDataIn[i + j * data.Stride + 2];
                    aaa /= 3;
                    bitmapDataout[i + j * data.Stride] =
                        bitmapDataout[i + j * data.Stride + 1] =
                        bitmapDataout[i + j * data.Stride + 2] =
                        aaa < wBTH ? byte.MinValue : byte.MaxValue;
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public double Median(List<double> numbers)
        {
            if (numbers.Count == 0)
                return 0;

            numbers = numbers.OrderBy(n => n).ToList();

            var halfIndex = numbers.Count() / 2;

            if (numbers.Count() % 2 == 0)
                return (numbers[halfIndex] + numbers[halfIndex - 1]) / 2.0;

            return numbers[halfIndex];
        }

        static double standardDeviation(IEnumerable<double> sequence)
        {
            double result = 0;

            if (sequence.Any())
            {
                double average = sequence.Average();
                double sum = sequence.Sum(d => Math.Pow(d - average, 2));
                result = Math.Sqrt((sum) / sequence.Count());
            }
            return result;
        }

        public Bitmap Bernsen(Bitmap bitmap, int w = 2, int l = 15)
        {
            BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            for (int i = 0; i < bitmapDataout.Length; i++)
                bitmapDataout[i] = byte.MaxValue;

            for (int i = w; i < dx - w; i++)
            {
                for (int j = w; j < dy - w; j++)
                {
                    List<double> neighbours = GetMask(bitmapDataIn, i, j, w, data);

                    float wMin = (float)neighbours.Min();
                    float wMax = (float)neighbours.Max();

                    float wBTH = (wMin + wMax) / 2;
                    float localContrast = wMax - wMin;

                    int index = i + j * data.Stride;

                    float aaa = bitmapDataIn[index] +
                        bitmapDataIn[index + 1] +
                        bitmapDataIn[index + 2];
                    aaa /= 3;

                    // A
                    //bitmapDataout[index] =
                    //    bitmapDataout[index + 1] =
                    //    bitmapDataout[index + 2] =
                    //    aaa < wBTH ? byte.MinValue : byte.MaxValue;

                    // B
                    if (localContrast < l)
                    {
                        bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] =
                        wBTH >= 128 ? byte.MaxValue : byte.MinValue;
                    }
                    else
                    {
                        bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] =
                        aaa > wBTH ? byte.MaxValue : byte.MinValue;
                    }
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public List<double> GetMask(byte[] bitmapData, int i, int j, int w, BitmapData data)
        {
            List<double> neighbours = new List<double>();
            // Extract the neighbourhood area
            for (int x = i - w; x < i + w; x++)
            {
                for (int y = j - w; y < j + w; y++)
                {
                    float bbb = bitmapData[x + y * data.Stride] + bitmapData[x + y * data.Stride + 1] + bitmapData[x + y * data.Stride + 2];
                    bbb /= 3;
                    neighbours.Add(bbb);
                }
            }
            return neighbours;
        }

        public byte GetMaskValue(byte[] bitmapData, int i, int j, int w, BitmapData data, float[,] mask)
        {
            byte value = 0;
            // Extract the neighbourhood area
            for (int x = i - w; x < i + w; x++)
            {
                for (int y = j - w; y < j + w; y++)
                {
                    byte pValue = (byte)((bitmapData[x + y * data.Stride] + bitmapData[x + y * data.Stride + 1] + bitmapData[x + y * data.Stride + 2]) / 3);
                    value += (byte)(pValue * mask[x - i + w, y - j + w]);
                }
            }
            return value;
        }

        public Bitmap FPixel(Bitmap bitmap, int w = 2)
        {
            BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            for (int i = 0; i < bitmapDataout.Length; i++)
                bitmapDataout[i] = byte.MaxValue;

            for (int i = w; i < dx - w; i++)
            {
                for (int j = w; j < dy - w; j++)
                {
                    List<double> neighbours = GetMask(bitmapDataIn, i, j, w, data);

                    int index = i + j * data.Stride;

                    bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] = (byte)neighbours.Average();
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public Bitmap FPixel2(Bitmap bitmap, int w = 30)
        {
            BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);

            //Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            //for (int i = 0; i < bitmapDataout.Length; i++)
            //    bitmapDataout[i] = byte.MaxValue;
            if (w == 0) w = 1;
            int dy = data.Height / w, dx = data.Width / w;
            byte[] bitmapDataout = new byte[data.Height * data.Stride / w /w];
            for (int j = 0; j < dy; j++)
            {
                for (int i = 0; i < dx + 1; i++)
                {
                    float sum = 0;
                    int count = 0;
                    for (int y = 0; y < w; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int kko = ((i * w + x) + (j * w + y) * (data.Width)) * 3;
                            if (kko >= bitmapDataIn.Length) break;
                            count++;

                            float a = bitmapDataIn[kko] +
                                bitmapDataIn[kko + 1] +
                                bitmapDataIn[kko + 2];
                            a /= 3;
                            sum += a;
                        }
                    }
                    sum /= count;

                    sum = Math.Min(sum, byte.MaxValue);



                    int kko2 = ((i) + (j) * (data.Width/w)) * 3;
                    if (kko2 >= bitmapDataout.Length) break;
                    bitmapDataout[kko2] =
                        bitmapDataout[kko2 + 1] =
                        bitmapDataout[kko2 + 2] = (byte)sum;
                }
            }
            Bitmap bitmap1 = new Bitmap(bitmap.Width/w, bitmap.Height/w);

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);
            LockBitmap(bitmap1, ref data);
            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap1.UnlockBits(data);

            return bitmap1;
        }

        public Bitmap FMedian(Bitmap bitmap, int w = 2)
        {
            BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            for (int i = 0; i < bitmapDataout.Length; i++)
                bitmapDataout[i] = byte.MaxValue;

            for (int i = w; i < dx - w; i++)
            {
                for (int j = w; j < dy - w; j++)
                {
                    List<double> neighbours = GetMask(bitmapDataIn, i, j, w, data);

                    int index = i + j * data.Stride;

                    bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] = (byte)Median(neighbours);
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public Bitmap Kuwahara(Bitmap bitmap, int w = 20)
        {
            if (w < 2) w = 2;
            Bitmap newbitmap = new Bitmap(bitmap.Width, bitmap.Height);



            int dy = bitmap.Height, dx = bitmap.Width;
            int[] MinX = { -(w / 2), 0, -(w / 2), 0 };
            int[] MaxX = { 0, w / 2, 0, w / 2 };
            int[] MinY = { -(w / 2), 0, -(w / 2), 0 }; ;
            int[] MaxY = { 0, w / 2, 0, w / 2 }; ;

            for (int x = 0; x < dx; x++)
            {
                for (int y = 0; y < dy; y++)
                {
                    int[] r = { 0, 0, 0, 0 };
                    int[] g = { 0, 0, 0, 0 };
                    int[] b = { 0, 0, 0, 0 };
                    int[] N = { 0, 0, 0, 0 };
                    int[] Maxr = { 0, 0, 0, 0 };
                    int[] Maxg = { 0, 0, 0, 0 };
                    int[] Maxb = { 0, 0, 0, 0 };
                    int[] Minr = { 255, 255, 255, 255 };
                    int[] Ming = { 255, 255, 255, 255 };
                    int[] Minb = { 255, 255, 255, 255 };

                    //cwiartki
                    for (int i = 0; i < 4; i++)
                    {
                        for (int x2 = MinX[i]; x2 < MaxX[i]; x2++)
                        {
                            int tempx = x + x2;
                            if (tempx >= 0 && tempx < dx)
                            {
                                for (int y2 = MinY[i]; y2 < MaxY[i]; y2++)
                                {
                                    int tempy = y + y2;
                                    if (tempy >= 0 && tempy < dy)
                                    {
                                        System.Drawing.Color color = bitmap.GetPixel(tempx, tempy);
                                        r[i] += color.R;
                                        g[i] += color.G;
                                        b[i] += color.B;
                                        ///////////////////////////
                                        if (color.R > Maxr[i])
                                        {
                                            Maxr[i] = color.R;
                                        }
                                        else if (color.R < Minr[i])
                                        {
                                            Minr[i] = color.R;
                                        }
                                        //////////////////////
                                        if (color.G > Maxg[i])
                                        {
                                            Maxg[i] = color.G;
                                        }
                                        else if (color.G < Ming[i])
                                        {
                                            Ming[i] = color.G;
                                        }
                                        ///////////////////////////////
                                        if (color.B > Maxb[i])
                                        {
                                            Maxb[i] = color.B;
                                        }
                                        else if (color.B < Minb[i])
                                        {
                                            Minb[i] = color.B;
                                        }
                                        ++N[i];
                                    }
                                }
                            }
                        }
                    }
                    int j = 0;
                    int mindiff = 10000;
                    for (int i = 0; i < 4; i++)
                    {
                        int currdiff = (Maxr[i] - Minr[i]) + (Maxg[i] - Ming[i]) + (Maxb[i] - Minb[i]);
                        if (currdiff < mindiff && N[i] > 0)
                        {
                            j = i;
                            mindiff = currdiff;
                        }
                    }

                    System.Drawing.Color newpixel = System.Drawing.Color.FromArgb(r[j] / N[j], g[j] / N[j], b[j] / N[j]);
                    newbitmap.SetPixel(x, y, newpixel);
                }

            }

            return newbitmap;
        }

        private void KMM(object sender, RoutedEventArgs e)
        {
            Szkieletyzacja szkieletyzacja = new Szkieletyzacja();
            szkieletyzacja.KMM(BitmapResult);
            szkieletyzacja.ShowDialog();
        }

        private void K3M(object sender, RoutedEventArgs e)
        {
            Szkieletyzacja szkieletyzacja = new Szkieletyzacja();
            szkieletyzacja.K3M(BitmapResult);
            szkieletyzacja.ShowDialog();
        }
    }
}

