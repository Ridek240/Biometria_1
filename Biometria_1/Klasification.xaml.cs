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
        Random random = new Random();
        private DataCenter dataCenter = new DataCenter();
        public Klasification()
        {
            InitializeComponent();
            //Losowanko(random);
            Randomizer();
        }

        private void Randomizer()
        {
            List<List<KeyStroke>> keyStrokes = GetRandom2(0.05f, 0.10f);
            List<List<OutputData>> resultKnn = knn(keyStrokes, KNNDistanceEucl);
            List<List<OutputData>> resultNB = NativeBayes(keyStrokes);
            KNN.Text = ShowData(resultKnn, keyStrokes[0]);
            NB.Text = ShowData(resultNB, keyStrokes[0]);
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
        public List<List<KeyStroke>> GetRandom2(float testingSize)
        {
            return GetRandom2(testingSize, 1 - testingSize);
        }
        public List<List<KeyStroke>> GetRandom2(float testingSize, float trainingSize)
        {
            List<List<KeyStroke>> result = new List<List<KeyStroke>>();

            List<KeyStroke> keyStrokes = dataCenter.GetKeyStrokesList();

            // Testing
            List<KeyStroke> testing = new List<KeyStroke>();
            for (int k = 0; k < GetSize() * testingSize; k++)
            {
                int r = random.Next(0, keyStrokes.Count);
                testing.Add(keyStrokes[r]);
                keyStrokes.RemoveAt(r);
            }
            result.Add(testing);

            // Training
            List<KeyStroke> training = new List<KeyStroke>();
            for (int k = 0; k < GetSize() * trainingSize; k++)
            {
                if (keyStrokes.Count <= 0) 
                    break;
                int r = random.Next(0, keyStrokes.Count);
                training.Add(keyStrokes[r]);
                keyStrokes.RemoveAt(r);
            }
            result.Add(training);

            return result;
        }
        // Gooood
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
        public List<List<OutputData>> NativeBayes(List<List<KeyStroke>> list) => NativeBayes(list[0], list[1]);

        public List<List<OutputData>> NativeBayes(List<KeyStroke> testing, List<KeyStroke> training)
        {
            List<List<OutputData>> output = new List<List<OutputData>>();

            NaiveBayes naiveBayes = new NaiveBayes(dataCenter);

            foreach (KeyStroke test in testing)
            {
                output.Add(naiveBayes.Get(test, training));
            }
            return output;
        }

        public double KNNDistanceEucl(KeyStroke Key1, KeyStroke Key2)
        {
            return Math.Pow(Key1.Val1 - Key2.Val1, 2) + Math.Pow(Key1.Val2 - Key2.Val2, 2);
        }

        public double KNNDistanceNormal(KeyStroke Key1, KeyStroke Key2)
            => Math.Abs(Key1.Length - Key2.Length);

        public double KNNDistanceMaximum(KeyStroke Key1, KeyStroke Key2)
            => (Key1.Val1 - Key2.Val1 > Key1.Val2 - Key2.Val2) ? Key1.Val1 - Key2.Val1 : Key1.Val2 - Key2.Val2;

        public double knnDistance4(KeyStroke Key1, KeyStroke Key2)
            => (Key1.Val1 - Key2.Val1 == Key1.Val2 - Key2.Val2) ? 0 : 1;

        public double knnDistance5(KeyStroke Key1, KeyStroke Key2)
            => (Key1.Val1 - Key2.Val1 == Key1.Val2 - Key2.Val2) ? 0 : 1;

        public double KNNDistanceDiscrete(KeyStroke Key1, KeyStroke Key2)
            => (Key1.Val1 - Key2.Val1 == Key1.Val2 - Key2.Val2) ? 0 : 1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Losowanko(random);
            Randomizer();
        }

        public string ShowData(List<List<OutputData>> data, List<KeyStroke> testing)
        {
            string result = "";
            int i = 0;
            foreach (List<OutputData> test in data)
            {
                result += i + " Testing key " + testing[i] + "\n";
                //double min = double.MaxValue;
                //OutputData minData = new OutputData();
                //foreach (OutputData data1 in test)
                //{
                //    if (min > data1.distance)
                //    {
                //        minData = data1;
                //        min = data1.distance;
                //    }
                //}
                int j = 0;
                foreach (OutputData data1 in test.OrderBy(p => p.distance))
                {
                    if (j++ > 5) break;
                    result += data1 + "\n";
                }
                //result += minData + ", \n\n";
                result += "\n";
                i++;
            }

            return result;
        }


        // Baaaaaaaaaaad
        //private void Losowanko(Random random)
        //{
        //    KeyStroke key = GetRandom(random.Next(0, GetSize()));
        //    List<KeyStroke> keys = GetRandom(0.05f);
        //    N.Content = key.Key;
        //    V1.Content = key.Val1;
        //    V2.Content = key.Val2;
        //    List<OutputGroup> Group1 = knn(keys);
        //    //List<OutputGroup> Group2 = NaiveBayes(keys);
        //    KNN.Text = ShowData(Group1, 20);
        //    //NB.Content = ShowData(Group2, 20);
        //}

        //public List<OutputGroup> NaiveBayes(List<KeyStroke> keys)
        //{
        //    List<OutputGroup> Group = new List<OutputGroup>();

        //    NaiveBayes naiveBayes = new NaiveBayes(dataCenter);

        //    foreach (KeyStroke key in keys)
        //    {
        //        foreach (DataGroup dataGroup in dataCenter.dataGroups)
        //        {
        //            if (key == null) continue;
        //            bool flag = false;
        //            foreach (KeyStroke keyStroke in dataGroup.keyStrokes)
        //            {
        //                foreach (KeyStroke k in keys)
        //                {
        //                    if (keyStroke == k) flag = true;
        //                }
        //            }
        //            if (flag) continue;
        //            OutputGroup output = new OutputGroup(dataGroup.Name);
        //            double distance = naiveBayes.Get(key, dataGroup.Name);
        //            output.distances.Add(distance);
        //            Group.Add(output);
        //        }
        //    }

        //    return Group;
        //}

        //public List<KeyStroke> GetRandom(float part)
        //{
        //    List<KeyStroke> result = new List<KeyStroke>();
        //    for (int k = 0; k < GetSize() * part; k++)
        //    {
        //        int count = 0;
        //        int r = random.Next(0, GetSize());
        //        for (int i = 0; i < dataCenter.dataGroups.Count; i++)
        //        {
        //            for (int j = 0; j < dataCenter.dataGroups[i].keyStrokes.Count; j++)
        //            {
        //                if (count == r) result.Add(dataCenter.dataGroups[i].keyStrokes[j]);
        //                count++;
        //            }
        //        }
        //    }
            
        //    return result;
        //}

        //public KeyStroke GetRandom(int random)
        //{
        //    int count = 0;
        //    for (int i = 0; i < dataCenter.dataGroups.Count; i++)
        //    {
        //        for (int j = 0; j < dataCenter.dataGroups[i].keyStrokes.Count; j++)
        //        {
        //            if (count == random) return dataCenter.dataGroups[i].keyStrokes[j];
        //            count++;
        //        }
        //    }
        //    return null;
        //}

        //public string ShowData(List<OutputGroup> Group, int k = 5)
        //{

        //    //find k smallest
        //    List<Output> Smallest = new List<Output>();
        //    double Min = int.MaxValue;
        //    OutputGroup MinGroup = null;
        //    for (int i = 0; i < k; i++)
        //    {
        //        Min = int.MaxValue;
        //        MinGroup = null;
        //        foreach (OutputGroup group in Group)
        //        {
        //            foreach (double distance in group.distances)
        //            {
        //                if (distance < Min)
        //                {
        //                    Min = distance;
        //                    MinGroup = group;
        //                }
        //            }
        //        }
        //        if (MinGroup != null)
        //        {
        //            double minreplace = Min;
        //            MinGroup.distances.Remove(Min);
        //            Smallest.Add(new Output(minreplace, MinGroup.name));
        //        }
        //    }
        //    List<Output> GroupOutput = new List<Output>();
        //    //Output wyj = new Output();
        //    foreach (Output output1 in Smallest)
        //    {
        //        Output wyj = GroupOutput.Find(e => e.name == output1.name);
        //        if (wyj == null)
        //        {
        //            GroupOutput.Add(new Output(1, output1.name, output1.Distance));
        //        }
        //        else
        //        {
        //            wyj.Distance++;
        //        }

        //    }

        //    double maximum = 0;
        //    double a = 0;
        //    string returnstring = "brak w grupie";
        //    foreach (Output common in GroupOutput)
        //    {
        //        if (maximum < common.Distance)
        //        {
        //            maximum = common.Distance;
        //            a = common.a;
        //            returnstring = common.name;
        //        }
        //    }
        //    return returnstring + " %" + a.ToString();
        //}


        //public List<OutputGroup> knn(List<KeyStroke> keys)
        //{
        //    List<OutputGroup> Group = new List<OutputGroup>();

        //    foreach (KeyStroke key in keys)
        //    {
        //        foreach (DataGroup dataGroup in dataCenter.dataGroups)
        //        {
        //            OutputGroup output = new OutputGroup(dataGroup.Name);
        //            foreach (KeyStroke keystroke in dataGroup.keyStrokes)
        //            {
        //                bool flag = false;
        //                foreach (KeyStroke k in keys)
        //                {
        //                    if (keystroke == k) flag = true;
        //                }
        //                if (flag) continue;
        //                double distance = Math.Sqrt(Math.Pow(key.Val1 - keystroke.Val1, 2) + Math.Pow(key.Val2 - keystroke.Val2, 2));
        //                output.distances.Add(distance);
        //            }
        //            Group.Add(output);
        //        }
        //    }
        //    return Group;
        //}

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

    //public class Output
    //{
    //    public double Distance;
    //    public string name;
    //    public double a;
    //    public Output(double Distance, string name)
    //    {
    //        this.Distance = Distance;
    //        this.name = name;
    //    }
    //    public Output(double Distance, string name, double s)
    //    {
    //        this.Distance = Distance;
    //        this.name = name;
    //        a = s;
    //    }
    //    public Output()
    //    { }
    //}
    //public class OutputGroup
    //{
    //    public string name;
    //    public List<double> distances = new List<double>();
    //    public OutputGroup(string name)
    //    {
    //        this.name = name;
    //    }
    //}

}
