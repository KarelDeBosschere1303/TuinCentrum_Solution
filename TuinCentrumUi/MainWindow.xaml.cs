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
    public partial class MainWindow : Window
    {
        private readonly TuinCentrumManager tuinCentrumManager;
        private readonly ITuinCentrumRepository tuinCentrumRepository;
        private readonly IFileProcessor fileProcessor;

        public MainWindow()
        {
            InitializeComponent();
            string connectionstring = @"Data Source=Workmate\SQLEXPRESS;Initial Catalog=Tuincetrum_B;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            tuinCentrumRepository = new TuinCentrumRepository(connectionstring);
            fileProcessor = new FileProcessor(tuinCentrumRepository);
            tuinCentrumManager = new TuinCentrumManager(fileProcessor, tuinCentrumRepository);
            LoadData();
        }

        private void LoadData()
        {
            var offertes = tuinCentrumRepository.GetOffertes();
            OffertesListView.ItemsSource = offertes;
            KlantenListView.ItemsSource = tuinCentrumRepository.GetKlanten();
        }

        private void KlantenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KlantenListView.SelectedItem is Klant selectedKlant)
            {
                MessageBox.Show($"Geselecteerde klant: {selectedKlant.Naam}");
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
            var newOfferWindow = new TuinCentrum_UI.NewOfferWindow();
            newOfferWindow.ShowDialog();
        }
        
        private void SearchKlantButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekTerm = KlantZoekTextBox.Text.ToLower();
            var gefilterdeKlanten = tuinCentrumRepository.GetKlanten()
                .Where(k => k.Naam.ToLower().Contains(zoekTerm) || k.Adres.ToLower().Contains(zoekTerm)).ToList();
            KlantenListView.ItemsSource = gefilterdeKlanten;
        }

        private void SearchOfferteButton_Click(object sender, RoutedEventArgs e)
        {
            string zoekTerm = OfferteZoekTextBox.Text.ToLower();
            var gefilterdeOffertes = tuinCentrumRepository.GetOffertes()
                .Where(o => o.Id.ToString().Contains(zoekTerm) || KlantenListView.Items
                    .OfType<Klant>()
                    .Any(k => k.Id == o.KlantNummer && k.Naam.ToLower().Contains(zoekTerm)))
                .ToList();
            OffertesListView.ItemsSource = gefilterdeOffertes;
        }
        private void UpdateOfferteButton_Click(object sender, RoutedEventArgs e)
        {
        //    if (OffertesListView.SelectedItem is Offerte selectedOfferte)
        //    {
        //        var updateOfferWindow = new TuinCentrum_UI.NewOfferWindow(selectedOfferte);
        //        updateOfferWindow.ShowDialog();
        //    }
        }

        // Andere methoden...


    }
}
