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
    public partial class NewOfferWindow : Window
    {
        private readonly TuinCentrumManager tuinCentrumManager;
        private List<ProductOfferteViewModel> productViewModels;
        private List<ProductOfferteViewModel> geselecteerdeProductenViewModels;

        public Offerte NieuweOfferte { get; private set; }

        public NewOfferWindow()
        {
            InitializeComponent();
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            var tuinCentrumRepository = new TuinCentrumRepository(connectionstring);
            IFileProcessor fileProcessor = new FileProcessor(tuinCentrumRepository);
            tuinCentrumManager = new TuinCentrumManager(tuinCentrumRepository, fileProcessor);

            LoadData();
        }

        private void LoadData()
        {
            var producten = tuinCentrumManager.GetAllProducten();
            productViewModels = producten.Select(p => new ProductOfferteViewModel
            {
                Id = p.Id,
                Naam = p.Naam,
                WetenschappelijkeNaam = p.WetenschappelijkeNaam,
                Prijs = p.Prijs,
                Beschrijving = p.Beschrijving,
                Aantal = 0
            }).ToList();

            geselecteerdeProductenViewModels = new List<ProductOfferteViewModel>();

            ProductenDataGrid.ItemsSource = productViewModels;
            GeselecteerdeProductenDataGrid.ItemsSource = geselecteerdeProductenViewModels;
        }

        private void SearchProductButton_Click(object sender, RoutedEventArgs e)
        {
            var zoekTerm = ProductZoekTextBox.Text.ToLower();
            var gefilterdeProducten = productViewModels
                .Where(p => p.Naam.ToLower().Contains(zoekTerm) || p.WetenschappelijkeNaam.ToLower().Contains(zoekTerm))
                .ToList();

            ProductenDataGrid.ItemsSource = gefilterdeProducten;
        }

        private void UpdateTotalPrice()
        {
            var geselecteerdeProducten = geselecteerdeProductenViewModels
                .Where(p => p.Aantal > 0)
                .ToDictionary(p => new Product(p.Id, p.Naam, p.WetenschappelijkeNaam, p.Prijs, p.Beschrijving), p => p.Aantal);

            var nieuweOfferte = new Offerte
            {
                Klant = new Klant { Id = int.Parse(KlantNummerTextBox.Text) },
                Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                Afhaal = AfhaalCheckBox.IsChecked ?? false,
                Aanleg = AanlegCheckBox.IsChecked ?? false,
                Producten = geselecteerdeProducten
            };

            decimal totalePrijs = nieuweOfferte.BerekenTotaleKostPrijs();
            TotalePrijsTextBlock.Text = totalePrijs.ToString("C");
        }

        private void VoegProductToeButton_Click(object sender, RoutedEventArgs e)
        {
            var geselecteerdeProducten = ProductenDataGrid.SelectedItems.Cast<ProductOfferteViewModel>().ToList();

            if (geselecteerdeProducten != null && geselecteerdeProducten.Count > 0)
            {
                foreach (var product in geselecteerdeProducten)
                {
                    string aantalString = Microsoft.VisualBasic.Interaction.InputBox($"Hoeveel {product.Naam} wil je toevoegen?", "Aantal Invoeren", "1");
                    if (int.TryParse(aantalString, out int aantal) && aantal > 0)
                    {
                        var existingProduct = geselecteerdeProductenViewModels.FirstOrDefault(p => p.Id == product.Id);
                        if (existingProduct != null)
                        {
                            existingProduct.Aantal += aantal;
                        }
                        else
                        {
                            geselecteerdeProductenViewModels.Add(new ProductOfferteViewModel
                            {
                                Id = product.Id,
                                Naam = product.Naam,
                                WetenschappelijkeNaam = product.WetenschappelijkeNaam,
                                Prijs = product.Prijs,
                                Beschrijving = product.Beschrijving,
                                Aantal = aantal
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ongeldig aantal. Probeer het opnieuw.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                GeselecteerdeProductenDataGrid.ItemsSource = null;
                GeselecteerdeProductenDataGrid.ItemsSource = geselecteerdeProductenViewModels;
                UpdateTotalPrice();
            }
            else
            {
                MessageBox.Show("Selecteer een product om toe te voegen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerwijderGeselecteerdeProductenButton_Click(object sender, RoutedEventArgs e)
        {
            var geselecteerdeProducten = GeselecteerdeProductenDataGrid.SelectedItems.Cast<ProductOfferteViewModel>().ToList();

            if (geselecteerdeProducten != null)
            {
                foreach (var product in geselecteerdeProducten)
                {
                    geselecteerdeProductenViewModels.Remove(product);
                }

                GeselecteerdeProductenDataGrid.ItemsSource = null;
                GeselecteerdeProductenDataGrid.ItemsSource = geselecteerdeProductenViewModels;
                UpdateTotalPrice();
            }
            else
            {
                MessageBox.Show("Selecteer een product om te verwijderen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(KlantNummerTextBox.Text, out int klantNummer))
            {
                // Check if the customer exists
                Klant klant = tuinCentrumManager.GetKlantById(klantNummer);
                if (klant == null)
                {
                    // If the customer does not exist, create a new customer
                    klant = new Klant { Id = klantNummer };
                }

                var geselecteerdeProducten = geselecteerdeProductenViewModels
                    .Where(p => p.Aantal > 0)
                    .ToDictionary(p => new Product(p.Id, p.Naam, p.WetenschappelijkeNaam, p.Prijs, p.Beschrijving), p => p.Aantal);

                NieuweOfferte = new Offerte
                {
                    Klant = klant,
                    Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                    Afhaal = AfhaalCheckBox.IsChecked ?? false,
                    Aanleg = AanlegCheckBox.IsChecked ?? false,
                    Producten = geselecteerdeProducten
                };

                // Bereken de totale kostprijs
                NieuweOfferte.KostPrijs = NieuweOfferte.BerekenTotaleKostPrijs();

                // Save the new offer to the database
                try
                {
                    tuinCentrumManager.AddOfferte(NieuweOfferte);
                    MessageBox.Show("Offerte succesvol aangemaakt!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij het opslaan van de offerte: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vul alstublieft een geldig klantnummer in.", "Ongeldig klantnummer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}