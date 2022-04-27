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
        private OldWindow old;
        public List<Points> IllegalPixels = new List<Points>();
        public List<int> Deleting = new List<int>{3, 5, 7, 12, 13, 14, 15, 20,
21, 22, 23, 28, 29, 30, 31, 48,
52, 53, 54, 55, 56, 60, 61, 62,
63, 65, 67, 69, 71, 77, 79, 80,
81, 83, 84, 85, 86, 87, 88, 89,
91, 92, 93, 94, 95, 97, 99, 101,
103, 109, 111, 112, 113, 115, 116, 117,
118, 119, 120, 121, 123, 124, 125, 126,
127, 131, 133, 135, 141, 143, 149, 151,
157, 159, 181, 183, 189, 191, 192, 193,
195, 197, 199, 205, 207, 208, 209, 211,
212, 213, 214, 215, 216, 217, 219, 220,
221, 222, 223, 224, 225, 227, 229, 231,
237, 239, 240, 241, 243, 244, 245, 246,
247, 248, 249, 251, 252, 253, 254, 255};

        public Szkieletyzacja()
        {
            InitializeComponent();
            old = new OldWindow();
        }
        public Bitmap KMM(Bitmap bitmap)
        {

            //znalezienie czarnich pikseli
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    if (color == System.Drawing.Color.FromArgb(0, 0, 0))
                    {
                        IllegalPixels.Add(new Points(x, y, 1));
                    }
                }
            }


            //setting 2


            foreach (Points point in IllegalPixels)
            {
                if (!IllegalPixels.Exists(z => z.x == (point.x - 1) && z.y == point.y) || !IllegalPixels.Exists(z => z.x == (point.x + 1) && z.y == point.y) || !IllegalPixels.Exists(z => z.x == (point.x) && z.y == point.y - 1) || !IllegalPixels.Exists(z => z.x == point.x && z.y == point.y + 1))
                {
                    point.weight = 2;
                }
            }

            //setting 3
            foreach (Points point in IllegalPixels)
            {
                if (point.weight == 1)
                    if (!IllegalPixels.Exists(z => z.x == point.x - 1 && z.y == point.y - 1) || !IllegalPixels.Exists(z => z.x == (point.x - 1) && z.y == point.y + 1) || !IllegalPixels.Exists(z => z.x == point.x + 1 && z.y == point.y - 1) || !IllegalPixels.Exists(z => z.x == point.x + 1 && z.y == point.y + 1))
                    {
                        point.weight = 3;
                    }

            }

            //setting 4
            
          //  foreach (Points point in IllegalPixels)
          //  {
          //      if (point.weight > 1)
          //      {
          //          int count = 0;
          //          for (int i = -1; i <= 1; i++)
          //          {
          //              for (int j = -1; j <= 1; j++)
          //              {
          //                  if (i == 0 && j == 0)
          //                  {
          //                      continue;
          //                  }
          //                  if (!IllegalPixels.Exists(z => z.x == point.x + i && z.y == point.y + j))
          //                  {
          //                      count++;
          //                  }
          //              }
          //          }
          //          if (count >= 2 && count <= 4)
          //          {
          //              point.weight = 4;
          //          }
          //      }
          //  }

            

          List<Points> deletingPixels = new List<Points>();
           
     //  //deleting 4
     //     foreach (Points point in IllegalPixels)
     //     {
     //         if(point.weight==4)
     //         {
     //             deletingPixels.Add(point);
     //         }
     //     }
     //     foreach (Points point in deletingPixels)
     //     {
     //         IllegalPixels.Remove(point);
     //     }

            
            int length = IllegalPixels.Count;
            for (int ha = 2; ha <= 3; ha++)
            {
                for (int i = 0; i < length; i++)
                {
                    Points point = IllegalPixels.FirstOrDefault(x => x.weight == ha%2+2);
                    if (point == null) continue;
                    int count = 0;
                    if(IllegalPixels.Exists(z => z.x == point.x && z.y == point.y +1))
                    {
                        count += 1;
                    }
                    if (IllegalPixels.Exists(z => z.x+1 == point.x && z.y == point.y + 1))
                    {
                        count += 2;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x + 1 && z.y == point.y ))
                    {
                        count += 4;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x + 1 && z.y == point.y - 1))
                    {
                        count += 8;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x && z.y == point.y - 1))
                    {
                        count += 16;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x - 1 && z.y == point.y - 1))
                    {
                        count += 32;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x -1 && z.y == point.y))
                    {
                        count += 64;
                    }
                    if (IllegalPixels.Exists(z => z.x == point.x -1 && z.y == point.y + 1))
                    {
                        count += 128;
                    }
                    if (Deleting.Exists(x => x == count))
                    {
                        IllegalPixels.Remove(point);
                    }
                    else point.weight = 1;
                }
            }

                Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap2))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            foreach (Points point in IllegalPixels)
            {
                bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 0));
            }

            ScelResult.Source = old.ConvertToImage(bitmap2);


                return bitmap;

        }

    }

    public class Points
    {
        public int weight;
        public int x;
        public int y;
        public Points(int x,int y, int weight)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
        }
    }
        

}
