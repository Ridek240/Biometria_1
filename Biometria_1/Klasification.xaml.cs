﻿using System;
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
        Random random = new Random();
        private DataCenter dataCenter = new DataCenter();
        public Klasification()
        {
            InitializeComponent();
            //Random random = new Random();
            //Losowanko(random);
            Randomizer();
        }

        private void Randomizer()
        {
            List<List<KeyStroke>> keyStrokes = GetRandom2(0.05f);
            List<List<OutputData>> resultKnn = knn(keyStrokes, knnDistance1);
            KNN.Text = ShowData(resultKnn, keyStrokes[0]);
        }

        private void Losowanko(Random random)
        {
            KeyStroke key = GetRandom(random.Next(0, GetSize()));
            List<KeyStroke> keys = GetRandom(0.05f);
            N.Content = key.Key;
            V1.Content = key.Val1;
            V2.Content = key.Val2;
            List<OutputGroup> Group1 = knn(keys);
            //List<OutputGroup> Group2 = NaiveBayes(keys);
            KNN.Text = ShowData(Group1, 20);
            //NB.Content = ShowData(Group2, 20);
        }

        public int GetSize()
        {
            int count = 0;
            for (int i = 0; i < dataCenter.dataGroups.Count; i++)
            {
                for (int j = 0; j < dataCenter.dataGroups[i].keyStrokes.Count; j++)
                {
                    count++;
                }
            }
            return count;
        }

        public List<KeyStroke> GetRandom(float part)
        {
            List<KeyStroke> result = new List<KeyStroke>();
            for (int k = 0; k < GetSize() * part; k++)
            {
                int count = 0;
                int r = random.Next(0, GetSize());
                for (int i = 0; i < dataCenter.dataGroups.Count; i++)
                {
                    for (int j = 0; j < dataCenter.dataGroups[i].keyStrokes.Count; j++)
                    {
                        if (count == r) result.Add(dataCenter.dataGroups[i].keyStrokes[j]);
                        count++;
                    }
                }
            }
            
            return result;
        }

        public List<List<KeyStroke>> GetRandom2(float part)
        {
            List<List<KeyStroke>> result = new List<List<KeyStroke>>();

            List<KeyStroke> keyStrokes = dataCenter.GetKeyStrokesList();

            // Testing
            List<KeyStroke> testing = new List<KeyStroke>();
            for (int k = 0; k < GetSize() * part; k++)
            {
                int r = random.Next(0, keyStrokes.Count);
                testing.Add(keyStrokes[r]);
                keyStrokes.RemoveAt(r);
            }
            result.Add(testing);

            // Training
            result.Add(keyStrokes);

            return result;
        }

        public KeyStroke GetRandom(int random)
        {
            int count = 0;
            for (int i = 0; i < dataCenter.dataGroups.Count; i++)
            {
                for (int j = 0; j < dataCenter.dataGroups[i].keyStrokes.Count; j++)
                {
                    if (count == random) return dataCenter.dataGroups[i].keyStrokes[j];
                    count++;
                }
            }
            return null;
        }

        public string ShowData(List<OutputGroup> Group, int k = 5)
        {

            //find k smallest
            List<Output> Smallest = new List<Output>();
            double Min = int.MaxValue;
            OutputGroup MinGroup = null;
            for (int i = 0; i < k; i++)
            {
                Min = int.MaxValue;
                MinGroup = null;
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
                    double minreplace = Min;
                    MinGroup.distances.Remove(Min);
                    Smallest.Add(new Output(minreplace, MinGroup.name));
                }
            }
            List<Output> GroupOutput = new List<Output>();
            //Output wyj = new Output();
            foreach (Output output1 in Smallest)
            {
                Output wyj = GroupOutput.Find(e => e.name == output1.name);
                if (wyj == null)
                {
                    GroupOutput.Add(new Output(1, output1.name, output1.Distance));
                }
                else
                {
                    wyj.Distance++;
                }

            }

            double maximum = 0;
            double a = 0;
            string returnstring = "brak w grupie";
            foreach (Output common in GroupOutput)
            {
                if (maximum < common.Distance)
                {
                    maximum = common.Distance;
                    a = common.a;
                    returnstring = common.name;
                }
            }
            return returnstring + " %" + a.ToString();
        }


        public List<OutputGroup> knn(List<KeyStroke> keys)
        {
            List<OutputGroup> Group = new List<OutputGroup>();

            foreach (KeyStroke key in keys)
            {
                foreach (DataGroup dataGroup in dataCenter.dataGroups)
                {
                    OutputGroup output = new OutputGroup(dataGroup.Name);
                    foreach (KeyStroke keystroke in dataGroup.keyStrokes)
                    {
                        bool flag = false;
                        foreach (KeyStroke k in keys)
                        {
                            if (keystroke == k) flag = true;
                        }
                        if (flag) continue;
                        double distance = Math.Sqrt(Math.Pow(key.Val1 - keystroke.Val1, 2) + Math.Pow(key.Val2 - keystroke.Val2, 2));
                        output.distances.Add(distance);
                    }
                    Group.Add(output);
                }
            }
            return Group;
        }

        public string ShowData(List<List<OutputData>> data, List<KeyStroke> testing)
        {
            string result = "";
            int i = 0;
            foreach (List<OutputData> test in data)
            {
                result += i + " Testing key " + testing[i] + "\n";
                double min = double.MaxValue;
                OutputData minData = new OutputData();
                foreach (OutputData data1 in test)
                {
                    if (min > data1.distance)
                    {
                        minData = data1;
                        min = data1.distance;
                    }
                }
                result += minData + ", \n\n";
                i++;
            }

            return result;
        }

        public List<List<OutputData>> knn(List<List<KeyStroke>> list, knnDistance knnDistance) => knn(list[0], list[1], knnDistance);

        public delegate double knnDistance(KeyStroke Key1, KeyStroke Key2);
        public List<List<OutputData>> knn(List<KeyStroke> testing, List<KeyStroke> training, knnDistance knnDistance)
        {
            List<List<OutputData>> output = new List<List<OutputData>>();

            foreach (KeyStroke test in testing)
            {
                List<OutputData> list = new List<OutputData>();
                foreach (KeyStroke train in training)
                {
                    OutputData outputData = new OutputData
                    {
                        distance = Math.Sqrt(knnDistance(test, train)),
                        KeyStroke = train
                    };
                    list.Add(outputData);
                }
                output.Add(list);
            }
            return output;
        }

        public double knnDistance1(KeyStroke Key1, KeyStroke Key2)
        {
            return Math.Pow(Key1.Val1 - Key2.Val1, 2) + Math.Pow(Key1.Val2 - Key2.Val2, 2);
        }


        public List<OutputGroup> NaiveBayes(List<KeyStroke> keys)
        {
            List<OutputGroup> Group = new List<OutputGroup>();

            NaiveBayes naiveBayes = new NaiveBayes(dataCenter);

            foreach (KeyStroke key in keys)
            {
                foreach (DataGroup dataGroup in dataCenter.dataGroups)
                {
                    if (key == null) continue;
                    bool flag = false;
                    foreach (KeyStroke keyStroke in dataGroup.keyStrokes)
                    {
                        foreach (KeyStroke k in keys)
                        {
                            if (keyStroke == k) flag = true;
                        }
                    }
                    if (flag) continue;
                    OutputGroup output = new OutputGroup(dataGroup.Name);
                    double distance = naiveBayes.Get(key, dataGroup.Name);
                    output.distances.Add(distance);
                    Group.Add(output);
                }
            }

            return Group;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Losowanko(random);
            Randomizer();
        }
    }



    public class OutputData
    {
        public double distance;
        public KeyStroke KeyStroke;
        public override string ToString()
        {
            return "D: " + distance + " " + KeyStroke;
        }
    }

    public class Output
    {
        public double Distance;
        public string name;
        public double a;
        public Output(double Distance, string name)
        {
            this.Distance = Distance;
            this.name = name;
        }
        public Output(double Distance, string name, double s)
        {
            this.Distance = Distance;
            this.name = name;
            a = s;
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
