using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Klant
    {
        public int? Id { get; set; }
        public string Naam { get; set; }
        public string Adres { get; set; }

        public Klant(int? id, string naam, string adres)
        {
            Id = id;
            Naam = naam;
            Adres = adres;
        }
    }
}
