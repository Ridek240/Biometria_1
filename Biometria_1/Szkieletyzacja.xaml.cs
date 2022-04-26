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

    public partial class Szkieletyzacja : Window
    {
        public List<Points> IllegalPixels;
        public Szkieletyzacja()
        {
            InitializeComponent();
        }
        public Bitmap KMM(Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    if (color == System.Drawing.Color.FromArgb(255, 255, 255))
                    {
                        IllegalPixels.Add(x, y, 1);
                    }
                }
            }

            return bitmap;

        }

    }

    public class Points
    {
        int weight;
        int x;
        int y;
        public Points(int x,int y, int weight)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
        }
    }
        

}
