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
                int offerteCount = 0;

                // Lees alle productinformatie uit het tweede bestand en sla het op in een dictionary
                Dictionary<int, List<(int, int)>> offerteProducten = new Dictionary<int, List<(int, int)>>();

                using (StreamReader reader = new StreamReader(fileName2))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            int offerteId = int.Parse(parts[0]);
                            int productId = int.Parse(parts[1]);
                            int aantal = int.Parse(parts[2]);

                            if (!offerteProducten.ContainsKey(offerteId))
                            {
                                offerteProducten[offerteId] = new List<(int, int)>();
                            }
                            offerteProducten[offerteId].Add((productId, aantal));
                        }
                        else
                        {
                            throw new Exception("Ongeldige offerteproductregel: " + line);
                        }
                    }
                }

                // Haal alle producten op
                List<Product> allProducts = _tuinCentrumRepository.GetAllProducten();

                // Verwerk het offertebestand
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 6)
                        {
                            int offerteId = int.Parse(parts[0]);
                            DateTime datum = DateTime.Parse(parts[1]);
                            int klantnummer = int.Parse(parts[2]);
                            bool afhaal = bool.Parse(parts[3]);
                            bool aanleg = bool.Parse(parts[4]);

                            // Zorg ervoor dat klantnummer niet 0 is
                            if (klantnummer == 0)
                            {
                                throw new Exception("Klantnummer mag niet 0 zijn");
                            }

                            // Haal de productinformatie voor deze offerte op
                            Dictionary<Product, int> producten = new Dictionary<Product, int>();
                            if (offerteProducten.TryGetValue(offerteId, out var productLijst))
                            {
                                foreach (var (productId, aantal) in productLijst)
                                {
                                    Product product = allProducts.Find(p => p.Id == productId);
                                    if (product != null)
                                    {
                                        producten.Add(product, aantal);
                                    }
                                }
                            }

                            // Voeg de offerte met producten toe aan de lijst
                            offertes.Add(new Offerte(datum, klantnummer, afhaal, aanleg, producten));
                            offerteCount++;
                            Console.WriteLine($"Offerte {offerteCount} logged.");
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


