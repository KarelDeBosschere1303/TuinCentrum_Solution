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
        public TuinCentrumManager(IFileProcessor fileProcessor, ITuinCentrumRepository tuinCentrumRepository)
        {
            _fileProcessor = fileProcessor;
            _tuinCentrumRepository = tuinCentrumRepository;
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
    }
}



