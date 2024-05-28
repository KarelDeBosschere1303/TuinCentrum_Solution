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
        private readonly TuinCentrumManager tuinCentrumManager;
        private readonly ITuinCentrumRepository tuinCentrumRepository;
        private readonly IFileProcessor fileProcessor;
        public Offerte GewijzigdeOfferte { get; private set; }
        private Dictionary<Product, int> productAantallen = new Dictionary<Product, int>();

        public EditOfferWindow(int offerteId)
        {
            InitializeComponent();
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            tuinCentrumRepository = new TuinCentrumRepository(connectionstring);
            fileProcessor = new FileProcessor(tuinCentrumRepository);
            tuinCentrumManager = new TuinCentrumManager(fileProcessor, tuinCentrumRepository);
            LoadData(offerteId);
        }

        private void LoadData(int offerteId)
        {
            var offerte = tuinCentrumRepository.GetOfferteById(offerteId);
            if (offerte != null)
            {
                OfferteIdTextBox.Text = offerte.Id.ToString();
                KlantNummerTextBox.Text = offerte.KlantNummer.ToString();
                DatumDatePicker.SelectedDate = offerte.Datum;
                AfhaalCheckBox.IsChecked = offerte.Afhaal;
                AanlegCheckBox.IsChecked = offerte.Aanleg;

                var productenViewModel = offerte.Producten.Select(p => new ProductOfferteViewModel
                {
                    ProductId = p.Key.Id,
                    Naam = p.Key.Naam,
                    WetenschappelijkeNaam = p.Key.WetenschappelijkeNaam,
                    Prijs = p.Key.Prijs,
                    Beschrijving = p.Key.Beschrijving,
                    Aantal = p.Value
                }).ToList();
                ProductenDataGrid.ItemsSource = productenViewModel;

                CalculateTotalPrice();
            }
            else
            {
                MessageBox.Show("Offerte niet gevonden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (var product in ProductenDataGrid.ItemsSource as List<ProductOfferteViewModel>)
            {
                totalPrice += product.Prijs * product.Aantal;
            }
            TotalePrijsTextBlock.Text = totalPrice.ToString("C");
            return totalPrice;
        }

        private void CalculateTotalPriceButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(KlantNummerTextBox.Text, out int klantNummer))
            {
                var geselecteerdeProducten = new Dictionary<Product, int>();

                foreach (var productViewModel in ProductenDataGrid.ItemsSource as List<ProductOfferteViewModel>)
                {
                    if (productViewModel.Aantal > 0)
                    {
                        var product = new Product(
                            productViewModel.ProductId,
                            productViewModel.Naam,
                            productViewModel.WetenschappelijkeNaam,
                            productViewModel.Prijs,
                            productViewModel.Beschrijving
                        );
                        geselecteerdeProducten.Add(product, productViewModel.Aantal);
                    }
                }

                GewijzigdeOfferte = new Offerte
                {
                    Id = int.Parse(OfferteIdTextBox.Text),
                    KlantNummer = klantNummer,
                    Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                    Afhaal = AfhaalCheckBox.IsChecked ?? false,
                    Aanleg = AanlegCheckBox.IsChecked ?? false,
                    Producten = geselecteerdeProducten,
                    KostPrijs = CalculateTotalPrice() // Bereken en stel de totale prijs in
                };

                // Save the updated offer to the database
                try
                {
                    tuinCentrumRepository.UpdateOfferte(GewijzigdeOfferte);
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
    }
}
