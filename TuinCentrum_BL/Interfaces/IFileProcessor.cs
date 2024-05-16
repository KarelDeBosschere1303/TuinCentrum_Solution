using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Interfaces;

namespace TuinCentrum_BL.Interfaces
{
    public interface IFileProcessor
    {
        List<Klant> LeesKlanten(string filename);
        List<Product>LeesProducten(string filename);
        List<Offerte>LeesOffertes(string filename);
        List<string> LeesOffertes_Producten(string filename);
    }
}
