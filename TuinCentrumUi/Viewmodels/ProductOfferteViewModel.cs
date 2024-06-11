using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Model;

namespace TuinCentrumUi.Viewmodels
{
        public class ProductQuantity
        {
            public Product Product { get; set; }
            public int Aantal { get; set; }

            public ProductQuantity(Product product, int aantal)
            {
                Product = product;
                Aantal = aantal;
            }
        }
    public class ProductOfferteViewModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string WetenschappelijkeNaam { get; set; }
        public decimal Prijs { get; set; }
        public string Beschrijving { get; set; }
        public int Aantal { get; set; }
    }

}
