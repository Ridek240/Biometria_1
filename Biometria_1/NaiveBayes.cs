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
        private int step = 50;
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

        public float GetProbability(int value, List<int> attribute)
        {
            int count = 0;
            for (int i = 0; i < attribute.Count; i++)
            {
                if (ConvertValue(attribute[i]) == ConvertValue(value)) count++;
            }

            return (float)count / attribute.Count;
        }

        public float GetProbabilityOfGroups()
        {
            return 1f / DataCenter.dataGroups.Count;
        }

        public float Get(KeyStroke keyStroke, string name)
        {
            List<List<int>> valueList = DataCenter.GetValuesLists();

            float Pc = GetProbabilityOfGroups();
            float Pxc = 1;
            float Px = 1;

            DataGroup dataGroup = DataCenter.GetDataGroup(name);
            List<List<int>> groupValueList = dataGroup.GetAttributes(keyStroke);

            for (int i = 0; i < DataCenter.AttributesCount; i++)
            {
                Pxc *= GetProbability(keyStroke[i], groupValueList[i]);
            }

            for (int i = 0; i < DataCenter.AttributesCount; i++)
            {
                Px *= GetProbability(keyStroke[i], valueList[i]);
            }

            return Pxc * Pc / Px;
        }


    }
}
