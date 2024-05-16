using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Product
    {
        private string naam;
        public string Naam { get; set;}
        private string wetenschappelijkeNaam;
        public string WetenschappelijkeNaam { get; set; }
        private decimal prijs;
        public decimal Prijs { get; set; }
        private string beschrijving;
        public string Beschrijving { get; set; }

        public Product(string naam, string wetenschappelijkeNaam, decimal prijs, string beschrijving)
        {
            Naam = naam;
            WetenschappelijkeNaam = wetenschappelijkeNaam;
            Prijs = prijs;
            Beschrijving = beschrijving;
        }
    }
}
