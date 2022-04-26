using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    class DataCenter
    {
        public List<DataGroup> dataGroups = new List<DataGroup>();
        public int AttributesCount = 2;

        public DataCenter()
        {
            string[] files = Directory.GetFiles("../../../DataKeyStrokes");

            for (int i = 0; i < files.Length; i++)
            {
                string[] lines = System.IO.File.ReadAllLines(files[i]);
                DataGroup dataGroup = new DataGroup();
                dataGroup.Name = files[i];
                foreach (string line in lines)
                {
                    string[] subs = line.Split(',');

                    if (subs.Length < 2) continue;
                    subs[0] = String.Concat(subs[0].Where(c => !Char.IsWhiteSpace(c)));
                    KeyStroke keyStroke = new KeyStroke
                    {
                        Key = subs[0],
                        Val1 = int.Parse(subs[1]),
                        Val2 = int.Parse(subs[2]),
                        DataGroup = dataGroup
                    };
                    dataGroup.keyStrokes.Add(keyStroke);
                }
                dataGroups.Add(dataGroup);
            }
        }

        public List<KeyStroke> GetKeyStrokesList()
        {
            List<KeyStroke> result = new List<KeyStroke>();
            foreach (DataGroup group in dataGroups)
            {
                foreach (KeyStroke keyStroke in group.keyStrokes)
                {
                    result.Add(keyStroke);
                }
            }
            return result;
        }

        public DataGroup GetDataGroup(string name)
        {
            for (int i = 0; i < dataGroups.Count; i++)
            {
                if (dataGroups[i].Name.CompareTo(name) == 0) return dataGroups[i];
            }
            return null;
        }

        public List<List<int>> GetValuesLists()
        {
            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < AttributesCount; i++)
            {
                List<int> attribute = new List<int>();
                result.Add(attribute);
            }
            for (int j = 0; j < dataGroups.Count; j++)
            {
                for (int k = 0; k < dataGroups[j].keyStrokes.Count; k++)
                {
                    for (int i = 0; i < AttributesCount; i++)
                    {
                        result[i].Add(dataGroups[j].keyStrokes[k][i]);
                    }
                }
            }

            return result;
        }
    }
}
