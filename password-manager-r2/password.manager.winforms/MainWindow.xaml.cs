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
using password.model;

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

        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            IServiceAsync service = new EncryptionService();
            try
            {
                decryptedTextBox.Text = await service.DecryptAsync(encryptedTextBox.Text);
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

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            IServiceAsync service = new EncryptionService();
            try
            {
                encryptedTextBox.Text = await service.EncryptAsync(plainTextBox.Text);
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

        private async void FindSiteButton_Click(object sender, RoutedEventArgs e)
        {
            SiteNotfoundlabel3.Content = string.Empty;
            IRepository repo = new XmlPersistence();
            Login login = repo.GetLogin(FindSiteTextBox.Text);
            if(login != null)
            {
                SiteTextBox.Text = login.Site;
                UserNameTextBox.Text = login.UserName;
                PasswordTextBox.Text = login.Password;
                if(AutoDecryptCheckBox.IsChecked == true)
                {
                    IServiceAsync service = new EncryptionService();
                    try
                    {
                        DecryptTextBox.Text = await service.DecryptAsync(login.Password);
                    }
                    catch (Exception ex)
                    {
                        PrintException(ex.Message);
                    }
                }
            }
            else
            {
                SiteNotfoundlabel3.Foreground = Brushes.Red;
                SiteNotfoundlabel3.Content = "Site not found";
            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateErrorlabel.Content = string.Empty;
            XmlPersistence repo = new XmlPersistence();
            Login login = new Login
            {
                Site = SiteTextBox.Text,
                UserName = UserNameTextBox.Text,
                Password = PasswordTextBox.Text
            };
            try
            {
                repo.Save(login);
            }
            catch (Exception ex)
            {
                UpdateErrorlabel.Foreground = Brushes.Red;
                UpdateErrorlabel.Content = ex.Message;
            }
            
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FindSiteTextBox.Clear();
        }

        private void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            SiteTextBox.Clear();
            UserNameTextBox.Clear();
            PasswordTextBox.Clear();
            DecryptTextBox.Clear();
        }
    }
}
