using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    class NaiveBayes
    {
        private DataCenter DataCenter;
        private int step = 25;
        public NaiveBayes(DataCenter dataCenter)
        {
            DataCenter = dataCenter;
        }

        public int GetAttributeValuesCount()
        {
            return 1000 / step;
        }

        public int ConvertValue(int value)
        {
            return (int)MathF.Floor((float)value / step);
        }

        public float GetProbability(int value, KeyStroke attribute)
        {
            int count = 0;
            for (int i = 0; i < KeyStroke.Count; i++)
            {
                if (ConvertValue(attribute[i]) == ConvertValue(value)) count++;
            }

            return (float)count / KeyStroke.Count;
        }

        public float GetProbability(int value, List<int> attribute)
        {
            int count = 0;
            for (int i = 0; i < attribute.Count; i++)
            {
                if (ConvertValue(attribute[i]) == ConvertValue(value)) count++;
            }

            return (float)count / attribute.Count;
        }

        public float GetProbabilityOfGroups(List<KeyStroke> training)
        {
            List<DataGroup> count = new List<DataGroup>();
            foreach (KeyStroke item in training)
            {
                count.Add(item.DataGroup);
            }
            count.Distinct();
            return 1f / count.Count;
        }

        public float Get(KeyStroke keyStroke, DataGroup dataGroup)
        {
            return Get(keyStroke, dataGroup, DataCenter.GetKeyStrokesList());
        }

        public float Get(KeyStroke keyStroke, DataGroup dataGroup, List<KeyStroke> training)
        {
            float Pc = GetProbabilityOfGroups(training);
            float Pxc = 1;
            float Px = 1;

            List<KeyStroke> groupValueList = dataGroup.GetAttributes(keyStroke, training);

            for (int i = 0; i < KeyStroke.Count; i++)
            {
                Pxc *= GetProbability(keyStroke[i], groupValueList[i]);
            }

            for (int i = 0; i < KeyStroke.Count; i++)
            {
                Px *= GetProbability(keyStroke[i], training[i]);
            }
            if (Px == 0) return 0;
            return Pxc * Pc / Px;
        }

        public List<OutputData> Get(KeyStroke keyStroke, List<KeyStroke> training)
        {
            List<OutputData> result = new List<OutputData>();
            foreach (KeyStroke train in training)
            {
                OutputData outputData = new OutputData
                {
                    KeyStroke = keyStroke,
                    distance = 1 - Get(keyStroke, train.DataGroup, training)
                };
                result.Add(outputData);
            }
            return result;
        }

    }
}
