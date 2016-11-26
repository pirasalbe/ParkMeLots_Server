using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerGarage
{
    class CDatabase
    {
        static int i = 1;
        public static int NewEntry()
        { return i++; }
    }
}
