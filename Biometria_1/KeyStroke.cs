using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria_1
{
    public class KeyStroke
    {
        public string Key;
        public int Val1;
        public int Val2;

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
    }
}
