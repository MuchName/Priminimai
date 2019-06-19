using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Priminimai
{
    public static class MasinosPavadinimoStringKeitimas
    {
        public static string TarpasBruksnelis(string pavadinimas)
        {

            pavadinimas = pavadinimas.Replace(" ", "_");

            return pavadinimas;
        }

        public static string BruksnelisTarpas(string pavadinimas)
        {
            pavadinimas = pavadinimas.Replace("_", " ");
            return pavadinimas;
        }
    }
}
