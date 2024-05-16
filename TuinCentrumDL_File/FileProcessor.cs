using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Model;

namespace TuinCentrumDL_File
{
    public class FileProcessor : IFileProcessor
    {
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
                        string[] parts = line.Split(';');
                        if (parts.Length == 3)
                        {
                            // Hier wordt het ID nog niet toegevoegd aan de klant
                             // Je kunt hier later de ID toevoegen
                            string naam = parts[0];
                            string adres = parts[1];
                            // Maak een nieuwe klant aan zonder ID
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
                        string[] parts = line.Split(';');
                        if (parts.Length == 4)
                        {
                            // Hier wordt het ID nog niet toegevoegd aan het product
                            int? id = null; // Je kunt hier later het ID toevoegen
                            string naam = parts[0];
                            string wetenschappelijkeNaam = parts[1];
                            decimal prijs = decimal.Parse(parts[2]);
                            string beschrijving = parts[3];
                            // Maak een nieuw product aan zonder ID

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
        public List<Offerte> LeesOffertes(string fileName)
        {
            try
            {
                List<Offerte> offertes = new List<Offerte>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length == 4)
                        {
                            // Hier wordt het ID nog niet toegevoegd aan de offerte
                           
                            DateTime datum = DateTime.Parse(parts[0]);
                            Klant klant = null; // Je kunt hier later de klant toevoegen
                            bool afhaal = bool.Parse(parts[2]);
                            bool aanleg = bool.Parse(parts[3]);
                            // Maak een nieuwe offerte aan zonder ID
                            Offerte offerte = new Offerte(datum, klant, afhaal, aanleg);
                            offertes.Add(offerte);
                        }
                        else
                        {
                            throw new Exception("Ongeldige offertesregel: " + line);
                        }
                    }
                }
                return offertes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij het lezen van offertesbestand {fileName}: {ex.Message}");
            }
        }
     

    }
}
