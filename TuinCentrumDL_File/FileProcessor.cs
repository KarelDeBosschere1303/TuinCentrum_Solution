using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Managers;

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

        public List<Offerte> LeesOffertes(string fileName, string fileName2)
        {
            try
            {
                List<Offerte> offertes = new List<Offerte>();

                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 6)
                        {
                            DateTime datum = DateTime.Parse(parts[1]);
                            int klantnummer = int.Parse(parts[2]);
                            bool afhaal = bool.Parse(parts[3]);
                            bool aanleg = bool.Parse(parts[4]);

                            // Zorg ervoor dat klantnummer niet 0 is
                            if (klantnummer == 0)
                            {
                                throw new Exception("Klantnummer mag niet 0 zijn");
                            }

                            // Lees de productinformatie uit het tweede bestand
                            var productLijnen = LeesOffertes_Producten(fileName2)
                                .Where(p => p.StartsWith(parts[0] + ","));

                            Dictionary<Product, int> producten = new Dictionary<Product, int>();

                            foreach (var productLijn in productLijnen)
                            {
                                string[] productParts = productLijn.Split(',');
                                if (productParts.Length == 3)
                                {
                                    int productId = int.Parse(productParts[1]);
                                    int aantal = int.Parse(productParts[2]);
                                    Product product = _tuinCentrumRepository.GetProductById(productId);

                                    if (product != null)
                                    {
                                        producten.Add(product, aantal);
                                        offertes.Add(new Offerte(datum, klantnummer, afhaal, aanleg, producten));
                                    }
                                    // else block removed
                                }
                                // else block removed
                            }

                        }
                        else
                        {
                            throw new Exception($"Ongeldige offertelijn: {line}");
                        }
                    }
                }

                return offertes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het lezen van offertebestand {fileName}: {ex.Message}", ex);
            }
        }


        public List<string> LeesOffertes_Producten(string fileName)
        {
            try
            {
                List<string> offerteProducten = new List<string>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            int offerteid = int.Parse(parts[0]);
                            int productid = int.Parse(parts[1]);
                            int aantal = int.Parse(parts[2]);
                            string offerteProduct = $"{offerteid},{productid},{aantal}";
                            offerteProducten.Add(offerteProduct);
                        }
                        else
                        {
                            throw new Exception("Ongeldige offerteproductregel: " + line);
                        }
                    }
                }
                return offerteProducten;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het lezen van offerteproductenbestand {fileName}: {ex.Message}");
            }
        }


    }
}


