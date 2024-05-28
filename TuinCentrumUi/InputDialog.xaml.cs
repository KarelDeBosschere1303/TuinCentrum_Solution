﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TuinCentrumUi
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
  
        public partial class InputDialog : Window
        {
            public InputDialog(string prompt, string title)
            {
                InitializeComponent();
                Prompt = prompt;
                Title = title;
                DataContext = this;
            }

            public string Prompt { get; set; }
            public string Answer => InputTextBox.Text;

            private void OkButton_Click(object sender, RoutedEventArgs e)
            {
                DialogResult = true;
                Close();
            }

            private void CancelButton_Click(object sender, RoutedEventArgs e)
            {
                DialogResult = false;
                Close();
            }
        }
    }

