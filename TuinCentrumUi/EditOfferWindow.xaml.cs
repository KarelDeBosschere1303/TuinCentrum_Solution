using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Managers;
using TuinCentrum_BL.Model;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;
using TuinCentrumUi.Viewmodels;

namespace TuinCentrumUi
{
    public partial class EditOfferWindow : Window
    {
        private Offerte _offerte;
        private List<ProductQuantity> productQuantities;
        private readonly TuinCentrumManager tuinCentrumManager;

        public EditOfferWindow(int offerteId)
        {
            InitializeComponent();
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            tuinCentrumManager = new TuinCentrumManager(new TuinCentrumRepository(connectionstring), new FileProcessor(new TuinCentrumRepository(connectionstring)));
            productQuantities = new List<ProductQuantity>();
            LoadOfferte(offerteId);
        }

        private void LoadOfferte(int offerteId)
        {
            // Hier roep je de methode aan die je offerte ophaalt
            _offerte = tuinCentrumManager.GetOfferteSById(offerteId);

            if (_offerte != null)
            {
                productQuantities = _offerte.Producten
                    .Select(p => new ProductQuantity(p.Key, p.Value))
                    .ToList();

                ProductenDataGrid.ItemsSource = productQuantities;

                // Vul de UI velden met de gegevens van de offerte
                OfferteIdTextBox.Text = _offerte.Id.ToString();
                KlantNummerTextBox.Text = _offerte.Klant.Id.ToString();
                DatumDatePicker.SelectedDate = _offerte.Datum;
                AfhaalCheckBox.IsChecked = _offerte.Afhaal;
                AanlegCheckBox.IsChecked = _offerte.Aanleg;
                UpdateTotalPrice();
            }
            else
            {
                MessageBox.Show("Offerte niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalPrice()
        {
            if (int.TryParse(KlantNummerTextBox.Text, out int klantNummer))
            {
                var geselecteerdeProducten = productQuantities.ToDictionary(p => p.Product, p => p.Aantal);

                var offerte = new Offerte
                {
                    Klant = new Klant { Id = klantNummer },
                    Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                    Afhaal = AfhaalCheckBox.IsChecked ?? false,
                    Aanleg = AanlegCheckBox.IsChecked ?? false,
                    Producten = geselecteerdeProducten
                };

                decimal totalePrijs = offerte.BerekenTotaleKostPrijs();
                TotalePrijsTextBlock.Text = totalePrijs.ToString("C");
            }
            else
            {
                TotalePrijsTextBlock.Text = "N.v.t.";
            }
        }

        private void CalculateTotalPriceButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_offerte != null)
            {
                if (int.TryParse(KlantNummerTextBox.Text, out int klantNummer))
                {
                    // Update de offerte met de nieuwe waarden van de UI-velden
                    _offerte.Klant = new Klant { Id = klantNummer };
                    _offerte.Datum = DatumDatePicker.SelectedDate ?? DateTime.Now;
                    _offerte.Afhaal = AfhaalCheckBox.IsChecked ?? false;
                    _offerte.Aanleg = AanlegCheckBox.IsChecked ?? false;
                    _offerte.Producten = productQuantities.ToDictionary(p => p.Product, p => p.Aantal);

                    // Bereken de totale kostprijs
                    _offerte.KostPrijs = _offerte.BerekenTotaleKostPrijs();

                    // Save the updated offer to the database
                    try
                    {
                        tuinCentrumManager.UpdateOfferte(_offerte);
                        MessageBox.Show("Offerte succesvol bijgewerkt!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fout bij het bijwerken van de offerte: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vul alstublieft een geldig klantnummer in.", "Ongeldig klantnummer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Offerte niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZoekProductButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekTerm = ZoekProductTextBox.Text.ToLower();
            var alleProducten = tuinCentrumManager.GetAllProducten();
            var gefilterdeProducten = alleProducten.Where(p => p.Naam.ToLower().Contains(zoekTerm)).ToList();
            AlleProductenDataGrid.ItemsSource = gefilterdeProducten;
        }

        private void VoegProductToeButton_Click(object sender, RoutedEventArgs e)
        {
            var geselecteerdeProducten = AlleProductenDataGrid.SelectedItems.Cast<Product>().ToList();

            if (geselecteerdeProducten != null && geselecteerdeProducten.Count > 0)
            {
                foreach (var product in geselecteerdeProducten)
                {
                    string aantalString = Microsoft.VisualBasic.Interaction.InputBox($"Hoeveel {product.Naam} wil je toevoegen?", "Aantal Invoeren", "1");
                    if (int.TryParse(aantalString, out int aantal) && aantal > 0)
                    {
                        var existingProduct = productQuantities.FirstOrDefault(p => p.Product.Id == product.Id);
                        if (existingProduct != null)
                        {
                            existingProduct.Aantal += aantal;
                        }
                        else
                        {
                            productQuantities.Add(new ProductQuantity(product, aantal));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ongeldig aantal. Probeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                ProductenDataGrid.ItemsSource = null;
                ProductenDataGrid.ItemsSource = productQuantities.ToList();
                UpdateTotalPrice();
            }
            else
            {
                MessageBox.Show("Selecteer een product om toe te voegen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerwijderGeselecteerdeProductenButton_Click(object sender, RoutedEventArgs e)
        {
            var geselecteerdeProducten = ProductenDataGrid.SelectedItems.Cast<ProductQuantity>().ToList();

            if (geselecteerdeProducten != null)
            {
                foreach (var productQuantity in geselecteerdeProducten)
                {
                    productQuantities.Remove(productQuantity);
                }

                ProductenDataGrid.ItemsSource = null;
                ProductenDataGrid.ItemsSource = productQuantities.ToList();
                UpdateTotalPrice();
            }
        }
    }
}
