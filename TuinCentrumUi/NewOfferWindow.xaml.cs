﻿using System;
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
        private readonly ITuinCentrumRepository tuinCentrumRepository;
        private readonly IFileProcessor fileProcessor;
        public Offerte NieuweOfferte { get; private set; }
        private Dictionary<Product, int> productAantallen = new Dictionary<Product, int>();

        public NewOfferWindow()
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
            var producten = tuinCentrumRepository.GetAllProducten();
            var productenViewModel = producten.Select(p => new ProductOfferteViewModel
            {
                ProductId = p.Id,
                Naam = p.Naam,
                WetenschappelijkeNaam = p.WetenschappelijkeNaam,
                Prijs = p.Prijs,
                Beschrijving = p.Beschrijving,
                Aantal = 0
            }).ToList();
            ProductenDataGrid.ItemsSource = productenViewModel;
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (var product in ProductenDataGrid.ItemsSource as List<ProductOfferteViewModel>)
            {
                totalPrice += product.Prijs * product.Aantal;
            }

            // Kortingen toepassen
            if (totalPrice > 5000)
            {
                totalPrice *= 0.90m; // 10% korting
            }
            else if (totalPrice > 2000)
            {
                totalPrice *= 0.95m; // 5% korting
            }

            // Leveringskosten toepassen
            if (!(AfhaalCheckBox.IsChecked ?? false)) // Als het niet afhaal is, dus levering
            {
                if (totalPrice < 500)
                {
                    totalPrice += 100; // Vaste kost van 100 euro voor levering onder 500 euro
                }
                else if (totalPrice < 1000)
                {
                    totalPrice += 50; // Vaste kost van 50 euro voor levering tussen 500 en 1000 euro
                }
                // Geen extra kosten voor levering boven 1000 euro
            }

            // Aanlegkosten toepassen
            if (AanlegCheckBox.IsChecked ?? false) // Als aanleg is geselecteerd
            {
                if (totalPrice > 5000)
                {
                    totalPrice *= 1.05m; // 5% kosten
                }
                else if (totalPrice > 2000)
                {
                    totalPrice *= 1.10m; // 10% kosten
                }
                else
                {
                    totalPrice *= 1.15m; // 15% kosten
                }
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

                NieuweOfferte = new Offerte
                {
                    KlantNummer = klantNummer,
                    Datum = DatumDatePicker.SelectedDate ?? DateTime.Now,
                    Afhaal = AfhaalCheckBox.IsChecked ?? false,
                    Aanleg = AanlegCheckBox.IsChecked ?? false,
                    Producten = geselecteerdeProducten,
                    KostPrijs = CalculateTotalPrice() // Bereken en stel de totale prijs in
                };

                // Save the new offer to the database
                try
                {
                    tuinCentrumRepository.AddOfferte(NieuweOfferte);
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
