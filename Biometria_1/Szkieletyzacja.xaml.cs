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
        public List<int> Deleting = new List<int>{
            3, 5, 7, 12, 13, 14, 15, 20,
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

        public List<int> deleting4 = new List<int> {3,6,12,24,48,96,192,129,7,14,
        28,65,112,224,193,131,15,30,60,120,240,225,195,135};

        public List<List<int>> ListA = new List<List<int>>
        {
            new List<int>
            {
                3, 6, 7, 12, 14, 15, 24, 28, 30, 31, 48, 56, 60, 62,
                63, 96, 112, 120, 124, 126, 127, 129, 131, 135,143,
                159, 191, 192, 193, 195, 199, 207, 223, 224, 225,
                227, 231, 239, 240, 241, 243, 247, 248, 249, 251,
                252, 253, 254
            },
            new List<int>
            {
                7, 14, 28, 56, 112, 131, 193, 224
            },
            new List<int>
            {
                7, 14, 15, 28, 30, 56, 60, 112, 120, 131, 135,
                193, 195, 224, 225, 240
            },
            new List<int>
            {
                7, 14, 15, 28, 30, 31, 56, 60, 62, 112, 120, 124,
                131, 135, 143, 193, 195, 199, 224, 225, 227, 240,
                241, 248
            },
            new List<int>
            {
                7, 14, 15, 28, 30, 31, 56, 60, 62, 63, 112, 120,
                124, 126, 131, 135, 143, 159, 193, 195, 199, 207,
                224, 225, 227, 231, 240, 241, 243, 248, 249, 252
            },
            new List<int>
            {
                7, 14, 15, 28, 30, 31, 56, 60, 62, 63, 112, 120,
                124, 126, 131, 135, 143, 159, 191, 193, 195, 199,
                207, 224, 225, 227, 231, 239, 240, 241, 243, 248,
                249, 251, 252, 254
            }

        };

        public Bitmap bitmap;

        public Szkieletyzacja()
        {
            InitializeComponent();
            old = new OldWindow();
        }
        public Bitmap KMM(Bitmap bitmap)
        {
            //znalezienie czarnich pikseli
            LokateBlack(bitmap);


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

            foreach (Points point in IllegalPixels)
            {
                if (point.weight > 1)
                {
                    int count = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }
                            if (IllegalPixels.Exists(z => z.x == point.x + i && z.y == point.y + j))
                            {
                                count++;
                            }
                        }
                    }
                    if (count >= 2 && count <= 4)
                    {
                        if (deleting4.Exists(x => x == CalculateValues(point)))
                            point.weight = 4;
                    }
                }
            }



            List<Points> deletingPixels = new List<Points>();

            //deleting 4
            foreach (Points point in IllegalPixels)
            {
                if (point.weight == 4)
                {
                    deletingPixels.Add(point);
                }
            }
            foreach (Points point in deletingPixels)
            {
                IllegalPixels.Remove(point);
            }

            deletingPixels.Clear();
            int length = IllegalPixels.Count;
            for (int ha = 2; ha <= 3; ha++)
            {
                for (int i = 0; i < length; i++)
                {
                    Points point = IllegalPixels.FirstOrDefault(x => x.weight == ha % 2 + 2);
                    if (point == null) continue;

                    if (Deleting.Exists(x => x == CalculateValues(point)))
                    {

                        IllegalPixels.Remove(point);
                    }
                    else point.weight = 1;
                }
            }
            foreach (Points point in deletingPixels)
            {
                IllegalPixels.Remove(point);
            }

            Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics gfx = Graphics.FromImage(bitmap2))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            foreach (Points point in IllegalPixels)
            {
                if (point.weight == 1)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 0));
                if (point.weight == 2)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 255));
                if (point.weight == 3)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 255, 0));
                if (point.weight == 4)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(255, 0, 0));
            }

            ScelResult.Source = old.ConvertToImage(bitmap2);


            return bitmap;

        }

        private void LokateBlack(Bitmap bitmap)
        {
            IllegalPixels.Clear();
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
        }

        public int CalculateValues(Points point)
        {
            int count = 0;
            if (IllegalPixels.Exists(z => z.x == point.x && z.y == point.y + 1))
            {
                count += 1;
            }
            if (IllegalPixels.Exists(z => z.x + 1 == point.x && z.y == point.y + 1))
            {
                count += 2;
            }
            if (IllegalPixels.Exists(z => z.x == point.x + 1 && z.y == point.y))
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
            if (IllegalPixels.Exists(z => z.x == point.x - 1 && z.y == point.y))
            {
                count += 64;
            }
            if (IllegalPixels.Exists(z => z.x == point.x - 1 && z.y == point.y + 1))
            {
                count += 128;
            }
            return count;
        }

        public void K3M(Bitmap bitmap)
        {
            LokateBlack(bitmap);
            //border 2 //deltion 3 //white 4
            //phase 0
           // Phase0();

            //phases 1..5



            for (int i = 1; i <= 5; i++)
            {
                Phase0();
                List<Points> White = new List<Points>();

                foreach (Points point in IllegalPixels)
                {
                    if (!(point.weight >= 2 && point.weight<=3)) continue;


                    if(point.weight==3 || ListA[i].Exists(z => z == CalculateValues(point)))
                    {
                        if(C2(point))
                        {
                            Points pointx = IllegalPixels.Find(z => z.x == point.x && z.y == point.y + 1);
                            if (ListA[i].Exists(z => z == CalculateValues(pointx)))
                            {
                                White.Add(pointx);
                                pointx.weight = 4;
                            }
                            else
                            {
                                point.weight = 4;
                                White.Add(point);
                            }
                        }
                        else
                        {
                            if (C3(point))
                            {
                                Points pointc3 = IllegalPixels.Find(z => z.x == point.x + 1 && z.y == point.y - 1);
                                if (ListA[i].Exists(z => z == CalculateValues(pointc3)))
                                {
                                    pointc3.weight = 3;
                                    White.Add(pointc3);
                                }
                                else
                                {
                                    point.weight = 4;
                                    White.Add(point);
                                }
                            }
                            else continue;
                        }
                    }
                    else 
                    { 
                        point.weight = 4;
                        White.Add(point);
                    }
                }

                foreach(Points point in White)
                {
                    IllegalPixels.Remove(point);
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
                if (point.weight == 1)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 0));
                if (point.weight == 2)
                    bitmap2.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 128));
 
            }

            ScelResult.Source = old.ConvertToImage(bitmap2);
        }

        //Condition C2: if W(x, y) == 241 and (x, y + 1) is
        //border and(x, y +2) == white and(x + 1, y + 2) == white,
        public bool C2(Points point)
        {
            if( CalculateValues(point)==241)
            {
                Points pointx = IllegalPixels.Find(z => z.x == point.x && z.y == point.y + 1);
                if (pointx == null || pointx.weight!=2) return false;
                Points pointsy2 = IllegalPixels.Find(z => z.x == point.x && z.y == point.y + 2);
                if(pointsy2 ==null || pointsy2.weight==4)
                {
                    Points pointsx1y2 = IllegalPixels.Find(z => z.x == point.x + 1 && z.y == point.y + 2);
                    if (pointsx1y2 == null || pointsx1y2.weight == 4) return true;
                }
            }
            return false;
        }


        //if (W(x, y) == 195 or W(x, y) == 227) and(x + 1, y −1) is border

        public bool C3(Points point)
        {
            if(CalculateValues(point)==195 || CalculateValues(point)==227)
            {
                Points pointc3 =  IllegalPixels.Find(z => z.x == point.x + 1 && z.y == point.y - 1);
                if (pointc3 != null && pointc3.weight == 2) return true;

            }
            return false;
        }
        private void Phase0()
        {
            foreach (Points point in IllegalPixels)
            {
                if (ListA[0].Exists(z => z == CalculateValues(point)))
                {
                    point.weight = 2;
                    Points point2 = IllegalPixels.Find(z => z.x == point.x - 1 && z.y == point.y - 1);
                    if (point2 != null && CalculateValues(point) == 193) point2.weight = 2;
                }
                else
                {
                    if (CalculateValues(point) == 95 && IllegalPixels.Exists(z => z.x == point.x - 2 && z.y == point.y))
                    {
                        point.weight = 2;
                    }
                    else if (CalculateValues(point) == 125 && IllegalPixels.Exists(z => z.x == point.x && z.y == point.y - 2))
                    {
                        point.weight = 2;
                    }
                    else if (CalculateValues(point) == 215 && IllegalPixels.Exists(z => z.x == point.x + 2 && z.y == point.y))
                    {
                        point.weight = 2;
                    }
                    else if (CalculateValues(point) == 245 && IllegalPixels.Exists(z => z.x == point.x && z.y == point.y + 2))
                    {
                        point.weight = 2;
                    }
                }
            }
            //phase 0a

            foreach (Points point in IllegalPixels)
            {
                if (point.weight == 2) continue;

                if (CalculateValues(point) == 31 || CalculateValues(point) == 124)
                {
                    point.weight = 2;
                }
            }
        }

        private void KMM(object sender, RoutedEventArgs e)
        {
            KMM(bitmap);
        }

        private void K3M(object sender, RoutedEventArgs e)
        {
            K3M(bitmap);
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
