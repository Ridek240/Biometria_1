using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    public class KeyStroke
    {
        public DataGroup DataGroup;
        public string Key;
        public int Val1;
        public int Val2;
        public static int Count = 2;

        public int this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return Val1;
                    case 1: return Val2;
                }
                return 0;
            }
        }

        public override string ToString()
        {
            return "G: " + DataGroup.Name + " K:" + Key;
        }

        public double Length
        {
            get => Math.Sqrt(Math.Pow(Val1, 2) + Math.Pow(Val2, 2));
        }
    }
}
