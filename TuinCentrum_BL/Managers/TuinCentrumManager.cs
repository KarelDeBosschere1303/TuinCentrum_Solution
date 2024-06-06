using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Exceptions;
using TuinCentrum_BL.Managers;



namespace TuinCentrum_BL.Managers
{
    public class TuinCentrumManager
    {
        
        private IFileProcessor _fileProcessor;
        private ITuinCentrumRepository _tuinCentrumRepository;
        public TuinCentrumManager(ITuinCentrumRepository tuinCentrumRepository,IFileProcessor fileProcessor)
        {
            _tuinCentrumRepository = tuinCentrumRepository;
            _fileProcessor = fileProcessor;

        }
        public void SchrijfKlanten(string filename)
        {
            List<Klant> klanten = _fileProcessor.LeesKlanten(filename);
            foreach (Klant klant in klanten)
            {
                if (!_tuinCentrumRepository.HeeftKlant(klant))
                {
                    _tuinCentrumRepository.SchrijfKlanten(klanten);
                }
            }
        }
        public void SchrijfProducten(string filename)
        {
            List<Product> producten = _fileProcessor.LeesProducten(filename);
            foreach (Product product in producten)
            {
                if (!_tuinCentrumRepository.HeeftProduct(product))
                {

                    _tuinCentrumRepository.SchrijfProducten(producten);

                }

            }
        }
        public void SchrijfOffertes(string filename, string filename2)
        {
            List<Offerte> offertes = _fileProcessor.LeesOffertes(filename, filename2);

            foreach (Offerte offerte in offertes)
            {
                if (!_tuinCentrumRepository.HeeftOfferte(offerte))
                {
                    _tuinCentrumRepository.SchrijfOffertes(new List<Offerte> { offerte });
                }
            }
        }
        public void AddOfferte(Offerte offerte)
        {
            if (!_tuinCentrumRepository.HeeftOfferte(offerte))
            {
                _tuinCentrumRepository.AddOfferte(offerte);
            }
            else
            {
                throw new DomeinException("Offerte bestaat al");
            }
        }
        public void UpdateOfferte(Offerte offerte)
        {
            if (_tuinCentrumRepository.HeeftOfferte(offerte))
            {
                _tuinCentrumRepository.UpdateOfferte(offerte);
            }
            else
            {
                throw new DomeinException("Offerte bestaat niet");
            }
        }

        public List<Product> GetAllProducten()
        {
            return _tuinCentrumRepository.GetAllProducten();
        }
        public Dictionary<int, Klant> GetKlanten()
        {
            return _tuinCentrumRepository.GetKlanten();
        }
        public Offerte GetOfferteSById(int offerteId)
        {
            return _tuinCentrumRepository.GetOfferteSById(offerteId);
        }

       public decimal BerekenTotaleKostPrijs(Offerte offerte)
        {
            return offerte.BerekenTotaleKostPrijs();
        }
        public Klant GetKlantById(int klantId)
        {
            return _tuinCentrumRepository.GetKlantById(klantId);
        }

    }
}



