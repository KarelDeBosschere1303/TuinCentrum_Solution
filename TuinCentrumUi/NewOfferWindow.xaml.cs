using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TuinCentrum_BL.Interfaces;
using TuinCentrum_BL.Managers;
using TuinCentrum_BL.Model;
using TuinCentrumDL_File;
using TuinCentrumDL_SQL;

namespace TuinCentrum_UI
{
    public partial class NewOfferWindow : Window
    {
        private List<Product> selectedProducts = new List<Product>(); // Lijst om geselecteerde producten bij te houden
        private readonly TuinCentrumManager tuinCentrumManager;
        private readonly ITuinCentrumRepository tuinCentrumRepository;
        private readonly IFileProcessor fileProcessor;
        public Offerte NieuweOfferte { get; private set; }

        public NewOfferWindow()
        {
            InitializeComponent();
            ProductenDataGrid.SelectionChanged += ProductenDataGrid_SelectionChanged; // Event handler toevoegen
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            tuinCentrumRepository = new TuinCentrumRepository(connectionstring);
            fileProcessor = new FileProcessor(tuinCentrumRepository);
            tuinCentrumManager = new TuinCentrumManager(fileProcessor, tuinCentrumRepository);
            // Voeg producten toe aan de datagrid
            LoadData();

        }
        private void LoadData()
        {
            ProductenDataGrid.ItemsSource = tuinCentrumRepository.GetAllProducten();
      
        }

        private void ProductenDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedProducts.Clear(); // Lijst leegmaken bij elke selectie om dubbele toevoegingen te voorkomen
            foreach (Product product in ProductenDataGrid.SelectedItems)
            {
                selectedProducts.Add(product); // Geselecteerde producten toevoegen aan de lijst
            }
            BerekenTotalePrijs(); // Totale prijs opnieuw berekenen wanneer productselectie verandert
        }

        private void BerekenTotalePrijs()
        {
            decimal totalePrijs = 0;
            foreach (Product product in selectedProducts)
            {
                totalePrijs += product.Prijs;
            }
            TotalePrijsTextBlock.Text = totalePrijs.ToString(); // Totale prijs weergeven in UI
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int klantNummer;
            if (int.TryParse(KlantNummerTextBox.Text, out klantNummer))
            {
                NieuweOfferte = new Offerte
                {
                    KlantNummer = klantNummer,
                    Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                    Afhaal = AfhaalCheckBox.IsChecked ?? false,
                    Aanleg = AanlegCheckBox.IsChecked ?? false,
                    KostPrijs = decimal.Parse(TotalePrijsTextBlock.Text) // Totale prijs uit de UI halen
                };

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Vul alstublieft een geldig klantnummer in.", "Ongeldig klantnummer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
