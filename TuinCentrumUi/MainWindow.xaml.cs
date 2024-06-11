using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Managers;
using TuinCentrum_BL.Model;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;
using TuinCentrumUi;
using TuinCentrumUi.Viewmodels;

namespace TuinCentrum_UI
{
    public partial class MainWindow : Window
    {
        private readonly TuinCentrumManager tuinCentrumManager;
        private List<Klant> allKlanten;
        private List<Offerte> allOffertes;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["TuinCentrumDB"].ConnectionString;
            ITuinCentrumRepository tuinCentrumRepository = new TuinCentrumRepository(connectionString);
            IFileProcessor fileProcessor = new FileProcessor(tuinCentrumRepository);
            tuinCentrumManager = new TuinCentrumManager(tuinCentrumRepository, fileProcessor);
            LoadData();
        }

        private void LoadData()
        {
            var klantenDict = tuinCentrumManager.GetKlanten();
            allKlanten = klantenDict.Values.ToList();
            allOffertes = allKlanten.SelectMany(k => k.Offertes).ToList();

            KlantenListView.ItemsSource = allKlanten;
            OffertesListView.ItemsSource = allOffertes;
        }

        private void SearchKlantButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekTerm = KlantZoekTextBox.Text.ToLower();
            var gefilterdeKlanten = allKlanten
                .Where(k => k.Naam.ToLower().Contains(zoekTerm) || k.Id.ToString().Equals(zoekTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            KlantenListView.ItemsSource = gefilterdeKlanten;
            KlantZoekTextBox.Text = string.Empty; // Clear the search box after search
        }

        private void SearchOfferteButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekTerm = OfferteZoekTextBox.Text.ToLower();
            var gefilterdeOffertes = allOffertes
                .Where(o => o.Id.ToString().Equals(zoekTerm, StringComparison.OrdinalIgnoreCase) ||
                            (o.Klant != null && o.Klant.Naam.ToLower().Contains(zoekTerm)))
                .ToList();

            OffertesListView.ItemsSource = gefilterdeOffertes;
            OfferteZoekTextBox.Text = string.Empty; // Clear the search box after search
        }

        private void KlantenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KlantenListView.SelectedItem is Klant selectedKlant)
            {
                string klantInfo = $"Naam: {selectedKlant.Naam}\nAdres: {selectedKlant.Adres}\nAantal Offertes: {selectedKlant.Offertes.Count}\n\nOffertes:\n";
                foreach (var offerte in selectedKlant.Offertes)
                {
                    klantInfo += $"Offerte ID: {offerte.Id}, Prijs: {offerte.KostPrijs}\n";
                }
                MessageBox.Show(klantInfo, "Klantinformatie");
            }
        }

        private void OffertesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OffertesListView.SelectedItem is Offerte selectedOfferte)
            {
                MessageBox.Show($"Geselecteerde offerte: {selectedOfferte.Id}");
            }
        }

        private void CreateOfferButton_Click(object sender, RoutedEventArgs e)
        {
            var newOfferWindow = new NewOfferWindow();
            newOfferWindow.ShowDialog();
            LoadData(); // Refresh the data after creating a new offer
        }

        private void EditOfferButton_Click(object sender, RoutedEventArgs e)
        {
            if (OffertesListView.SelectedItem is Offerte selectedOfferte)
            {
                var editOfferWindow = new EditOfferWindow(selectedOfferte.Id);
                editOfferWindow.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Selecteer een offerte om te bewerken.", "Geen offerte geselecteerd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
