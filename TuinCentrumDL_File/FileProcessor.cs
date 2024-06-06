using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Managers;
using TuinCentrum_BL.Exceptions;

namespace TuinCentrumDL_File
{
    public class FileProcessor : IFileProcessor
    {
        private ITuinCentrumRepository _tuinCentrumRepository;
        public FileProcessor(ITuinCentrumRepository tuinCentrumRepository)
        {
            _tuinCentrumRepository = tuinCentrumRepository;
            
            
        }
        public List<Klant> LeesKlanten(string fileName)
        {
            try
            {
                List<Klant> klanten = new List<Klant>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            string naam = parts[1];
                            string adres = parts[2];
                            Klant klant = new Klant(naam, adres);
                            klanten.Add(klant);
                        }
                        else
                        {
                            throw new Exception("Ongeldige klantenregel: " + line);
                        }
                    }
                }
                return klanten;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het lezen van klantenbestand {fileName}: {ex.Message}");
            }
        }

        public List<Product> LeesProducten(string fileName)
        {
            try
            {
                List<Product> producten = new List<Product>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 5)
                        {
                            int id = int.Parse(parts[0]);
                            string naam = parts[1];
                            string wetenschappelijkeNaam = parts[2];
                            decimal prijs = decimal.Parse(parts[3]);
                            string beschrijving = parts[4];
                            if (string.IsNullOrEmpty(naam))
                            {
                                continue; // Skip this product and go to the next line
                            }


                            producten.Add(new Product(id, naam, wetenschappelijkeNaam, prijs, beschrijving));

                        }

                        else
                        {
                            throw new Exception("Ongeldige productregel: " + line);
                        }
                    }
                }
                return producten;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het lezen van productenbestand {fileName}: {ex.Message}");
            }
        }

        public List<Offerte> LeesOffertes(string offerteFilePath, string offerteProductFilePath)
        {
            try
            {
                List<Offerte> offertes = new List<Offerte>();
                Dictionary<int, Klant> klantDict = _tuinCentrumRepository.GetKlanten();
                Dictionary<int, Product> productDict = _tuinCentrumRepository.GetAllProducten().ToDictionary(p => p.Id);

                // Lees offertes in
                using (var reader = new StreamReader(offerteFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var columns = line.Split('|');
                        if (columns.Length == 6)
                        {
                            int id = int.Parse(columns[0]);
                            DateTime datum = DateTime.Parse(columns[1]);
                            int klantId = int.Parse(columns[2]);
                            bool afhaal = !bool.Parse(columns[3]); // Omgedraaid wegens correctie leerkracht
                            bool aanleg = bool.Parse(columns[4]);
                            decimal kostprijs = decimal.Parse(columns[5]);

                            if (klantDict.TryGetValue(klantId, out Klant klant))
                            {
                                Offerte offerte = new Offerte(id, datum, klant, afhaal, aanleg, kostprijs);
                                offertes.Add(offerte);
                                Console.WriteLine($"Ingelezen offerte ID: {id}");
                            }
                            else
                            {
                                Console.WriteLine($"Klant ID {klantId} niet gevonden voor Offerte ID {id}");
                            }
                        }
                    }
                }

                // Lees offerte producten in en koppel ze aan de offertes
                using (var reader = new StreamReader(offerteProductFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var columns = line.Split('|');
                        if (columns.Length == 3)
                        {
                            int offerteId = int.Parse(columns[0]);
                            int productId = int.Parse(columns[1]);
                            int aantal = int.Parse(columns[2]);

                            var offerte = offertes.FirstOrDefault(o => o.Id == offerteId);
                            if (offerte != null)
                            {
                                if (productDict.TryGetValue(productId, out Product product))
                                {
                                    offerte.VoegProductToe(product, aantal);
                                    Console.WriteLine($"Ingelezen offerteProduct: OfferteID = {offerteId}, ProductID = {productId}, Aantal = {aantal}");
                                }
                                else
                                {
                                    Console.WriteLine($"Product ID {productId} niet gevonden voor Offerte ID {offerteId}");
                                }
                            }
                        }
                    }
                }

                return offertes;
            }
            catch (Exception ex)
            {
                throw new DomeinException($"FileProcessor-LeesOffertes, {ex.Message}");
            }
        }



    }
}


