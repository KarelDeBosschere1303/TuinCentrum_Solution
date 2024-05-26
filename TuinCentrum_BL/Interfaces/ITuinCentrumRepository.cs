using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Model;

namespace TuinCentrum_BL.Interfaces
{
    public interface ITuinCentrumRepository
    {
        bool HeeftKlant(Klant klant);
        bool HeeftProduct(Product product);
        bool HeeftOfferte(Offerte offerte);
        void SchrijfKlanten(List<Klant> klanten);
        void SchrijfProducten(List<Product> producten);
        void SchrijfOffertes(List<Offerte> offertes);
        Product GetProductById(int productId);
        List <Product> GetAllProducten();
        List <Klant> GetKlanten();
        List <Offerte> GetOffertes();




    }
}
