using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Offerte
    {
        private DateTime datum;
        public Klant Klant { get; set; }
        private bool afhaal;
        private bool aanleg;
        public Dictionary<Product, int> Producten { get; set;}

      public Offerte(DateTime datum, Klant klant, bool afhaal, bool aanleg)
        {
            Datum = datum;
            Klant = klant;
            Afhaal = afhaal;
            Aanleg = aanleg;
            Producten = new Dictionary<Product, int>();
        }

        public DateTime Datum
        {
            get { return datum; }
            set
            {
                if (value > DateTime.Now)
                    throw new Exception("Datum in de toekomst");
                datum = value;
            }
        }

        public bool Afhaal
        {
            get { return afhaal; }
            set { afhaal = value; }
        }

        public bool Aanleg
        {
            get { return aanleg; }
            set { aanleg = value; }
        }
    }

    
        
     
}
