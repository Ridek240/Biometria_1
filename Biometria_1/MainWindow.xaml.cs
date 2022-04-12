using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Biometria_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public readonly static string ImageFilePath = "../../../A.jpg";
        //public readonly static string ImageFilePath = "../../../text2.png";
        public readonly static string ImageFilePath = "../../../testowy.jpg";
        //public readonly static string ImageFilePath = "../../../Tree.jpg";
        public readonly static string ResultFilePath = "../../../Result.jpg";

        private bool ready = false;
        private bool isFloodType = false;

        private Bitmap BitmapSrc;
        private Bitmap BitmapResult;
        private OldWindow old;
        public MainWindow()
        {
            BitmapSrc = new Bitmap(ImageFilePath);
            BitmapResult = new Bitmap(BitmapSrc);
            InitializeComponent();
            ready = true;
            old = new OldWindow();
            UpdateImage();

            DataCenter dataCenter = new DataCenter();
        }

        private void OpenOld(object sender, RoutedEventArgs e)
        {
            
            old.ShowDialog();
        }

        private void UpdateImage()
        {
            ImageResult.Source = old.ConvertToImage(BitmapResult);
        }

        private void Magic_wand(object sender, MouseButtonEventArgs e)
        {
            double pozx = (int)e.GetPosition(ImageResult).X;
            double Realx = ImageResult.ActualWidth;
            int pozy = (int)e.GetPosition(ImageResult).Y;
            double Realy = ImageResult.ActualHeight;

            double scalex = pozx / Realx;
            double scaley = pozy / Realy;

            int bitmapx = BitmapSrc.Width;
            int bitmapy = BitmapSrc.Height;
            int pixelx = (int)(bitmapx * scalex);
            int pixely = (int)(bitmapy * scaley);
            System.Drawing.Color pixelC = BitmapSrc.GetPixel(pixelx, pixely);
            System.Drawing.Point pixelP = new System.Drawing.Point(pixelx, pixely);

            int limit = int.TryParse(Limit.Text, out limit) ? limit : 0;



            //BitmapResult = new Bitmap(BitmapSrc);
            if (isFloodType)
                BitmapResult = Blackout(pixelC, BitmapSrc, (int)Mask.Value);
            else
                BitmapResult = Segmentation(pixelP, BitmapSrc, (int)Mask.Value, limit);
            UpdateImage();
        }

        private static System.Drawing.Point[] neighbours =
        {
            new System.Drawing.Point(-1, 0),
            new System.Drawing.Point(1, 0),
            new System.Drawing.Point(0, 1),
            new System.Drawing.Point(0, -1)
        };

        private Bitmap Segmentation(System.Drawing.Point position, Bitmap bitmap, int range, int maxIteration = 0)
        {
            System.Drawing.Color blackpixel = System.Drawing.Color.FromArgb(0, 0, 0);
            System.Drawing.Color pixelCompateTo = bitmap.GetPixel(position.X, position.Y);

            Queue<System.Drawing.Point> pixels = new Queue<System.Drawing.Point>();
            pixels.Enqueue(position);

            bool[,] visited = new bool[bitmap.Width, bitmap.Height];
            Bitmap bitmapResult = new Bitmap(bitmap);

            int iteration = 0;
            while (pixels.Count > 0)
            {
                if (maxIteration > 0 && iteration++ >= maxIteration) break;
                System.Drawing.Point pixel = pixels.Dequeue();
                if (pixel.X < 0 || pixel.X >= bitmap.Width ||
                    pixel.Y < 0 || pixel.Y >= bitmap.Height ||
                    visited[pixel.X, pixel.Y]) continue;
                visited[pixel.X, pixel.Y] = true;

                if (GetTolerance(bitmap.GetPixel(pixel.X, pixel.Y), pixelCompateTo, range))
                {
                    bitmapResult.SetPixel(pixel.X, pixel.Y, blackpixel);
                    foreach (System.Drawing.Point neighbour in neighbours)
                    {
                        System.Drawing.Point nPixel = new System.Drawing.Point(
                            pixel.X + neighbour.X,
                            pixel.Y + neighbour.Y);
                        pixels.Enqueue(nPixel);
                    }
                }
            }

            return bitmapResult;
        }

        private bool GetTolerance(System.Drawing.Color currentPixel, System.Drawing.Color pixelCompateTo, int rangeMin, int rangeMax)
        {
            return currentPixel.R >= pixelCompateTo.R - rangeMin && currentPixel.R <= pixelCompateTo.R + rangeMax
                & currentPixel.G >= pixelCompateTo.G - rangeMin && currentPixel.G <= pixelCompateTo.G + rangeMax
                & currentPixel.B >= pixelCompateTo.B - rangeMin && currentPixel.B <= pixelCompateTo.B + rangeMax;
        }

        private bool GetTolerance(System.Drawing.Color currentPixel, System.Drawing.Color pixelCompateTo, int range)
        {
            return GetTolerance(currentPixel, pixelCompateTo, range, range);
        }

        private Bitmap Blackout(System.Drawing.Color pixel, Bitmap bitmap, int range)
        {
            System.Drawing.Color blackpixel = System.Drawing.Color.FromArgb(0, 0, 0);
            Bitmap bitmapResult = new Bitmap(bitmap);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color atpixel = BitmapSrc.GetPixel(x, y);
                    if (GetTolerance(atpixel, pixel, range))
                    {
                        bitmapResult.SetPixel(x, y, blackpixel);
                    }
                }
            }
            return bitmapResult;
        }

        private void Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Mask_label.Content = (int)Mask.Value;
        }

        private void Flood_Checked(object sender, RoutedEventArgs e)
        {
            isFloodType = Flood.IsChecked.Value;
        }
    }
}

