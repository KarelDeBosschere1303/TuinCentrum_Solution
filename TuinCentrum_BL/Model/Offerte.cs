using System;
using System.Collections.Generic;

namespace TuinCentrum_BL.Model
{
    public class Offerte
    {
        public string KlantNaam => Klant?.Naam;
        public int Id { get; set; }

        private DateTime datum;
        public DateTime Datum
        {
            get => datum;
            set
            {
                if (value == default)
                    throw new ArgumentException("Datum mag niet leeg zijn.");
                datum = value;
            }
        }

        
        public Klant Klant
        { get; set; }

        private bool afhaal;
        public bool Afhaal
        {
            get => afhaal;
            set => afhaal = value;
        }

        private bool aanleg;
        public bool Aanleg
        {
            get => aanleg;
            set => aanleg = value;
        }

        private Dictionary<Product, int> producten;
        public Dictionary<Product, int> Producten
        {
            get => producten;
            set => producten = value ?? throw new ArgumentNullException(nameof(Producten));
        }

        
        public decimal KostPrijs { get; set; }
        
        public Offerte()
        {
            Producten = new Dictionary<Product, int>();
        }

        public Offerte(int id, DateTime datum, Klant klant, bool afhaal, bool aanleg, decimal kostprijs)
        {
            Id = id;
            Datum = datum;
            Klant = klant;
            Afhaal = afhaal;
            Aanleg = aanleg;
            KostPrijs = kostprijs;
            Producten = new Dictionary<Product, int>();
        }

        public Offerte(DateTime datum, Klant klant, bool afhaal, bool aanleg, Dictionary<Product, int> producten)
        {
            Datum = datum;
            Klant = klant;
            Afhaal = afhaal;
            Aanleg = aanleg;
            Producten = producten;
            KostPrijs = BerekenTotaleKostPrijs();
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

        public void VoegProductToe(Product product, int aantal)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (Producten.ContainsKey(product))
            {
                Producten[product] += aantal;
            }
            else
            {
                Producten[product] = aantal;
            }
            KostPrijs = BerekenTotaleKostPrijs();
        }

        public void VerwijderProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (Producten.ContainsKey(product))
            {
                Producten.Remove(product);
            }
            KostPrijs = BerekenTotaleKostPrijs();
        }
    }
}
