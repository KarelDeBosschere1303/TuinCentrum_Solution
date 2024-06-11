using Xunit;
using TuinCentrum_BL.Model;
using TuinCentrum_BL.Managers;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;

namespace UnitTesting
{
    public class UnitTest1
    {
        [Fact]
        public void KlantConstructor_CreatesKlantWithCorrectValues()
        {
            // Arrange
            string naam = "Karel";
            string adres = "Wolfputstraat 1";

            // Act
            var klant = new Klant(naam, adres);

            // Assert
            Assert.Equal(naam, klant.Naam);
            Assert.Equal(adres, klant.Adres);
        }

        [Fact]
        public void ProductConstructor_CreatesProductWithCorrectValues()
        {
            // Arrange
            int id = 1;
            string naam = "Roos";
            string wetenschappelijkeNaam = "Rosa";
            decimal prijs = 10;
            string beschrijving = "Prachtige roos";

            // Act
            var product = new Product(id, naam, wetenschappelijkeNaam, prijs, beschrijving);

            // Assert
            Assert.Equal(id, product.Id);
            Assert.Equal(naam, product.Naam);
            Assert.Equal(wetenschappelijkeNaam, product.WetenschappelijkeNaam);
            Assert.Equal(prijs, product.Prijs);
            Assert.Equal(beschrijving, product.Beschrijving);
        }

        [Fact]
        public void OfferteConstructor_CreatesOfferteWithCorrectValues()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            bool afgehaald = false;
            bool aangelegd = false;
            var producten = new Dictionary<Product, int>();
            var now = DateTime.Now;

            // Act
            var offerte = new Offerte(now, klant, afgehaald, aangelegd, producten);

            // Assert
            Assert.Equal(now, offerte.Datum);
            Assert.Equal(klant, offerte.Klant);
            Assert.Equal(afgehaald, offerte.Afhaal);
            Assert.Equal(aangelegd, offerte.Aanleg);
            Assert.Equal(producten, offerte.Producten);
        }
        [Fact]
        public void VoegOfferteToe_VerhoogtAantalOffertesMet1()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var offertes = new List<Offerte>();
            var offerte = new Offerte(DateTime.Now, klant, false, false, new Dictionary<Product, int>());

            int oorspronkelijkeAantal = offertes.Count;

            // Act
            offertes.Add(offerte);

            // Assert
            Assert.Equal(oorspronkelijkeAantal + 1, offertes.Count);
        }
        [Fact]
        public void VoegProductToe_VerhoogtAantalProductenMet1()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var offerte = new Offerte(DateTime.Now, klant, false, false, new Dictionary<Product, int>());
            var product = new Product(1, "Roos", "Rosa", 10, "Prachtige roos");

            int oorspronkelijkeAantal = offerte.Producten.Count;

            // Act
            offerte.VoegProductToe(product, 1);

            // Assert
            Assert.Equal(oorspronkelijkeAantal + 1, offerte.Producten.Count);
            Assert.Equal(1, offerte.Producten[product]);
        }
        [Fact]
        public void VerwijderProduct_VerlaagtAantalProductenMetEen()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var offerte = new Offerte(DateTime.Now, klant, false, false, new Dictionary<Product, int>());
            var product = new Product(1, "Roos", "Rosa", 10, "Prachtige roos");

            offerte.VoegProductToe(product, 1);
            int oorspronkelijkeAantal = offerte.Producten.Count;

            // Act
            offerte.VerwijderProduct(product);

            // Assert
            Assert.Equal(oorspronkelijkeAantal - 1, offerte.Producten.Count);
        }

        [Fact]
        public void BerekenTotaleKostPrijs_ReturnsCorrectAmount()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var product1 = new Product(1, "Roos", "Rosa", 10, "Prachtige roos");
            var product2 = new Product(2, "Tulp", "Tulipa", 5, "Mooie tulp");
            var producten = new Dictionary<Product, int>
            {
                { product1, 2 },
                { product2, 3 }
            };
            var offerte = new Offerte(DateTime.Now, klant, false, false, producten);

            // Act
            decimal totaleKostPrijs = offerte.BerekenTotaleKostPrijs();

            // Assert
            Assert.Equal(135, totaleKostPrijs); // 20 + 15 + 100 = 35
        }

        [Fact]
        public void BerekenTotaleKostPrijs_IncludesAfhaalKosten()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var product1 = new Product(1, "Roos", "Rosa", 10, "Prachtige roos");
            var producten = new Dictionary<Product, int> { { product1, 1 } };
            var offerte = new Offerte(DateTime.Now, klant, false, false, producten);

            // Act
            decimal totaleKostPrijs = offerte.BerekenTotaleKostPrijs();

            // Assert
            Assert.Equal(110, totaleKostPrijs); // 10 (product) + 100 (verzendkosten)
        }

        [Fact]
        public void BerekenTotaleKostPrijs_IncludesAanlegKosten()
        {
            // Arrange
            var klant = new Klant("Karel", "Wolfputstraat 1");
            var product1 = new Product(1, "Roos", "Rosa", 1000, "Dure roos");
            var producten = new Dictionary<Product, int> { { product1, 1 } };
            var offerte = new Offerte(DateTime.Now, klant, false, true, producten);

            // Act
            decimal totaleKostPrijs = offerte.BerekenTotaleKostPrijs();

            // Assert
            Assert.Equal(1150, totaleKostPrijs); // 1000 (product) + 150 (aanlegkosten 15% van 1000)
        }
        [Fact]
        public void VergelijkProducten_MetVerschillendeIdZelfdeWaarden_ZijnWelGelijk()
        {
            // Arrange
            var product1 = new Product(1, "Roos", "Rosa", 10, "Prachtige roos");
            var product2 = new Product(2, "Roos", "Rosa", 10, "Prachtige roos");

            // Act & Assert
            Assert.Equal(product1, product2);
        }

        
    }
}