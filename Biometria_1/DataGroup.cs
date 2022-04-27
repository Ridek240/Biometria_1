using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    public class DataGroup
    {
        public string Name;
        public List<KeyStroke> keyStrokes = new List<KeyStroke>();
        public List<KeyStroke> GetAttributes(KeyStroke keyStroke)
        {
            return GetAttributes(keyStroke, keyStrokes);
        }
        public List<KeyStroke> GetAttributes(KeyStroke keyStroke, List<KeyStroke> training)
        {
            List<KeyStroke> attributes = new List<KeyStroke>();
            foreach (KeyStroke key in training)
            {
                if (key.DataGroup == keyStroke.DataGroup)
                    attributes.Add(key);
            }
            return attributes;
        }
    }
}
