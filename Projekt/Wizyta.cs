using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    class Wizyta
    {
        public int NumerWizyty;
        public int numerPrzypisany;

        public Wizyta(int numerPrzypisany, int numerWizyty)
        {
            this.numerPrzypisany = numerPrzypisany;
            NumerWizyty = numerWizyty;
        }
    }
}
