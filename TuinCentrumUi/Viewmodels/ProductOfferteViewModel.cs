using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrumUi.Viewmodels
{
    public class ProductOfferteViewModel
    {
        public int ProductId { get; set; }
        public string Naam { get; set; }
        public string WetenschappelijkeNaam { get; set; }
        public decimal Prijs { get; set; }
        public string Beschrijving { get; set; }
        public int Aantal { get; set; }
    }
}
