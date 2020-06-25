using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using password.service;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeButtonsState();
        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            IService service = new EncryptionService();
            try
            {
                decryptedTextBox.Text = service.Decrypt(encryptedTextBox.Text);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
            InitializeButtonsState();
        }

        private void PrintException(string message)
        {
            errorLabel.Foreground = Brushes.Red;
            errorLabel.Content = message;
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            IService service = new EncryptionService();
            try
            {
                encryptedTextBox.Text = service.Encrypt(plainTextBox.Text);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
            InitializeButtonsState();
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
            InitializeButtonsState();
        }

        private void InitializeData()
        {
            plainTextBox.Clear();
            encryptedTextBox.Clear();
            decryptedTextBox.Clear();
            errorLabel.Content = string.Empty;
        }

        private void InitializeButtonsState()
        {
            decryptButton.IsEnabled = !string.IsNullOrWhiteSpace(encryptedTextBox.Text);
            encryptButton.IsEnabled = !string.IsNullOrWhiteSpace(plainTextBox.Text);
        }

        private void plainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            InitializeButtonsState();
        }

        private void encryptedTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            InitializeButtonsState();
        }

        private void decryptedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeButtonsState();
        }

        private void decryptedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InitializeButtonsState();
        }
    }
}
