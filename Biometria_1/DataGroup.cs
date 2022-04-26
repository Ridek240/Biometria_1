using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    class DataGroup
    {
        public string Name;
        public List<KeyStroke> keyStrokes = new List<KeyStroke>();
        public int AttributesCount = 2;
        public List<List<int>> GetAttributes(KeyStroke keyStroke)
        {
            List<List<int>> attributes = new List<List<int>>();
            for (int i = 0; i < AttributesCount; i++)
            {
                attributes.Add(new List<int>());
            }
            for (int i = 0; i < keyStrokes.Count; i++)
            {
                for (int j = 0; j < AttributesCount; j++)
                {
                    attributes[j].Add(keyStrokes[i][j]);
                }
            }
            return attributes;
        }
    }
}
