using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Model
{
    public class Offerte
    {
        private int id;
        public int Id { get; set; }
        private DateTime datum;
        public DateTime Datum { get; set; }
        private int klantnummer;
        public int KlantNummer { get; set; }
        private bool afhaal;
        public bool Afhaal { get; set; }
        private bool aanleg;
        public bool Aanleg { get; set; }
        private Dictionary<Product, int> producten;
        public Dictionary<Product, int> Producten { get; set; }
        private decimal kostprijs;
        public decimal KostPrijs { get; set; }

        public Offerte() { }

        public Offerte(int id, DateTime datum, int klantnummer, bool afhaal, bool aanleg, decimal kostprijs)
        {
            Id = id;
            Datum = datum;
            KlantNummer = klantnummer;
            Afhaal = afhaal;
            Aanleg = aanleg;
            KostPrijs = kostprijs;

        }

        public Offerte(DateTime datum, int klantNummer, bool afhaal, bool aanleg,Dictionary<Product,int> producten)
        {
            Id = id;
            Datum = datum;
            KlantNummer = klantNummer;
            Afhaal = afhaal;
            Aanleg = aanleg;
            Producten = producten; // Add the missing initialization of the Producten dictionary
            KostPrijs = BerekenTotaleKostPrijs();
        }
        
        public void VoegProductToe(Product product, int aantal)
        {
            if (Producten.ContainsKey(product))
            {
                Producten[product] += aantal;
            }
            else
            {
                Producten.Add(product, aantal);
            }
        }

        public decimal BerekenTotaleKostPrijs()
        {
            decimal totaleKostprijs = 0;

            foreach (KeyValuePair<Product, int> kvp in Producten)
            {
                Product product = kvp.Key;
                int aantal = kvp.Value;

                decimal productKostprijs = product.Prijs * aantal;
                totaleKostprijs += productKostprijs;
            }

            totaleKostprijs -= BerekenKorting(totaleKostprijs);
            totaleKostprijs += BerekenVerzendKosten(totaleKostprijs);
            totaleKostprijs += BerekenAanlegKosten(totaleKostprijs);

            return totaleKostprijs;
        }

        private decimal BerekenKorting(decimal totaleKostprijs)
        {
            decimal korting = 0;

            if (totaleKostprijs > 5000)
            {
                korting = totaleKostprijs * 0.10m;
            }
            else if (totaleKostprijs > 2000)
            {
                korting = totaleKostprijs * 0.05m;
            }

            return korting;
        }

        private decimal BerekenVerzendKosten(decimal totaleKostprijs)
        {
            decimal verzendKosten = 0;

            if (!Afhaal)
            {
                if (totaleKostprijs < 500)
                {
                    verzendKosten = 100;
                }
                else if (totaleKostprijs >= 500 && totaleKostprijs < 1000)
                {
                    verzendKosten = 50;
                }
            }

            return verzendKosten;
        }

        private decimal BerekenAanlegKosten(decimal totaleKostprijs)
        {
            decimal aanlegKosten = 0;

            if (Aanleg)
            {
                if (totaleKostprijs < 2000)
                {
                    aanlegKosten = totaleKostprijs * 0.15m;
                }
                else if (totaleKostprijs >= 2000 && totaleKostprijs < 5000)
                {
                    aanlegKosten = totaleKostprijs * 0.10m;
                }
                else if (totaleKostprijs >= 5000)
                {
                    aanlegKosten = totaleKostprijs * 0.05m;
                }
            }

            return aanlegKosten;
        }
    }

    
        
     
}
