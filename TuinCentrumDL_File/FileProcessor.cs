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
                            int? id = null; // Je kunt hier later de ID toevoegen
                            string naam = parts[0];
                            string adres = parts[1];
                            // Maak een nieuwe klant aan zonder ID
                            Klant klant = new Klant(id, naam, adres);
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
                            decimal prijs = decimal.Parse(parts[1]);
                            int voorraad = int.Parse(parts[2]);                           
                            // Maak een nieuw product aan zonder ID
                            Product product = new Product(id, naam, prijs, voorraad);
                            producten.Add(product);
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


}
