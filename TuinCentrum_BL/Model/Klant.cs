using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Klant
    {
        private string naam;
        public string Naam { get; set; }
        private string adres;
        public string Adres { get; set; }

        public Klant(string naam, string adres)
        {
            Naam = naam;
            Adres = adres;
        }


    }
            
}
