using System.Collections.Generic;
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
        List<Product> GetAllProducten();
        Dictionary<int,Klant> GetKlanten();
        
        void AddOfferte(Offerte offerte);
        void UpdateOfferte(Offerte offerte);
        Offerte GetOfferteSById(int offerteId);
        Klant GetKlantById(int klantId);
    }
}
