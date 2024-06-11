using System;

namespace TuinCentrum_BL.Model
{
    public class Product
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

        private string wetenschappelijkeNaam;
        public string WetenschappelijkeNaam
        {
            get => wetenschappelijkeNaam;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Wetenschappelijke naam mag niet leeg zijn.");
                wetenschappelijkeNaam = value;
            }
        }

        private decimal prijs;
        public decimal Prijs
        {
            get => prijs;
            set
            {
                if (value <= 0) throw new ArgumentException("Prijs moet groter zijn dan 0.");
                prijs = value;
            }
        }

        private string beschrijving;
        public string Beschrijving
        {
            get => beschrijving;
            set => beschrijving = value;
        }

        public Product(int id, string naam, string wetenschappelijkeNaam, decimal prijs, string beschrijving)
        {
            Id = id;
            Naam = naam ?? throw new ArgumentNullException(nameof(naam));
            WetenschappelijkeNaam = wetenschappelijkeNaam ?? throw new ArgumentNullException(nameof(wetenschappelijkeNaam));
            Prijs = prijs;
            Beschrijving = beschrijving;
        }

        public override bool Equals(object obj)
        {
            if (obj is Product other)
            {
                return string.Equals(Naam, other.Naam, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(WetenschappelijkeNaam, other.WetenschappelijkeNaam, StringComparison.OrdinalIgnoreCase) &&
                       Prijs == other.Prijs &&
                       string.Equals(Beschrijving, other.Beschrijving, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

    }
}
