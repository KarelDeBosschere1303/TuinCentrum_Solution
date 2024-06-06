using System;
using System.Collections.Generic;

namespace TuinCentrum_BL.Model
{
    public class Klant
    {
        private int id;
        public int Id
        {
            get => id;
            set
            {
                if (value <= 0) throw new ArgumentException("Id moet groter zijn dan 0.");
                id = value;
            }
        }

        private string naam;
        public string Naam
        {
            get => naam;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Naam mag niet leeg zijn.");
                naam = value;
            }
        }

        private string adres;
        public string Adres
        {
            get => adres;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Adres mag niet leeg zijn.");
                adres = value;
            }
        }

        public List<Offerte> Offertes { get; set; } = new List<Offerte>();
        public int AantalOffertes => Offertes.Count;
        public Klant() { }

        public Klant(string naam, string adres)
        {  
            Naam = naam ?? throw new ArgumentNullException(nameof(naam));
            Adres = adres ?? throw new ArgumentNullException(nameof(adres));
        }
    }
}
