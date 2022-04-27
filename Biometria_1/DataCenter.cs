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
    }
}
