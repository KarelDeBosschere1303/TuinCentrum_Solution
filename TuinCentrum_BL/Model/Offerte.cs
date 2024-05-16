using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Offerte
    {
        public int? id;
        private DateTime? created;
        private Klant klant;
        public bool afhaal;
        public bool aanleg;
        private Dictionary<Product, int> producten;
    }
}
