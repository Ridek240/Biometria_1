using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logika interakcji dla klasy Klasification.xaml
    /// </summary>
    public partial class Klasification : Window
    {
        public Klasification()
        {
            InitializeComponent();
            string name = knn(56,256,null,20);
            int lol = 1;
        }



        public string knn( int val1 =0, int val2 =0, string key = null, int k=5)
        {
            DataCenter dataCenter = new DataCenter();
            List<OutputGroup> Group = new List<OutputGroup>();
            
            foreach(DataGroup dataGroup in dataCenter.dataGroups)
            {
                OutputGroup output = new OutputGroup(dataGroup.Name);
                foreach(KeyStroke keystroke in dataGroup.keyStrokes)
                {
                    double distance = Math.Sqrt(Math.Pow(val1 - keystroke.Val1, 2) + Math.Pow(val2 - keystroke.Val2, 2));
                    output.distances.Add(distance);
                }
                Group.Add(output);
            }


            //find k smallest
            List<Output> Smallest = new List<Output>();
            double Min = int.MaxValue;
            OutputGroup MinGroup = null;
            for (int i = 0; i < k; i++)
            {
                Min = int.MaxValue;
                foreach (OutputGroup group in Group)
                {
                    foreach (double distance in group.distances)
                    {
                        if (distance < Min)
                        {
                            Min = distance;
                            MinGroup = group;
                        }
                    }
                }
                if (MinGroup != null)
                {
                    MinGroup.distances.Remove(Min);
                    Smallest.Add(new Output(Min, MinGroup.name));
                }
            }
            List<Output> GroupOutput = new List<Output>(); 
            //Output wyj = new Output();
            foreach(Output output1 in Smallest)
            {
                Output wyj = GroupOutput.Find(e => e.name == output1.name);
                if(wyj==null)
                {
                    GroupOutput.Add(new Output(1, output1.name));
                }
                else
                {
                    wyj.Distance++;
                }
                
            }

            int maximum = 0;
            string returnstring = "brak w grupie";
            foreach(Output common in GroupOutput)
            {
                if(maximum<common.Distance)
                {
                    maximum = (int)common.Distance;
                    returnstring = common.name;
                }
            }
            return returnstring;
        }


    }
    public class Output
    {
        public double Distance;
        public string name;
        public Output(double Distance, string name)
        {
            this.Distance = Distance;
            this.name = name;
        }
        public Output()
        { }
    }
    public class OutputGroup
    {
        public string name;
        public List<double> distances = new List<double>();
        public OutputGroup(string name)
        {
            this.name = name;
        }
    }

}
