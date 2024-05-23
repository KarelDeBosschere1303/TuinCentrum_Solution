using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Klant
    {
        private int id;
        public int Id { get; set; }
        private string naam;
        public string Naam { get; set; }
        private string adres;
        public string Adres { get; set; }

        public Klant(string naam, string adres)
        {
            Id = id;
            Naam = naam ?? throw new ArgumentNullException(nameof(naam));
            Adres = adres ?? throw new ArgumentNullException(nameof(adres));
        }
    }
            
}
