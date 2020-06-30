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
using System.Timers;
using System.Net.Http.Headers;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IEnumerable<Login> logins;
        IRepository repo = new XmlPersistence();
        IServiceAsync service = new EncryptionService();
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeButtonsState();
            GetAllRecordsAsync();
        }

        #region private non UI methods 
        private void GetAllRecords()
        {
            System.Threading.Thread.Sleep(2500);
            logins = repo.GetLogins();
        }
        #endregion

        #region private UI coupled methods

        private async Task GetAllRecordsAsync()
        {
            await Task.Run(() =>
            {
                GetAllRecords();
            });
            SearchingIsReady(true);
        }

        private void SearchingIsReady(bool ready)
        {
            FindSiteButton.IsEnabled = ready;
        }

        private async Task Decrypt(string encryptedValue)
        {
            try
            {
                decryptedTextBox.Text = await service.DecryptAsync(encryptedValue);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private void PrintException(string message)
        {
            errorLabel.Foreground = Brushes.Red;
            errorLabel.Content = message;
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

        private async Task FindSite(string site)
        {
            var login = logins.Where(l => l.Site.StartsWith(site)).FirstOrDefault();
            await ShowLoginUI(login);
        }

        /// <summary>
        /// Show Login Model in TextBoxes
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private async Task ShowLoginUI(Login login)
        {
            ClearDataInputs();
            if (login == null) { SiteTextBox.Text = "Not found"; return; }
            SaveAlreadyEncryptedCheckBox.IsChecked = false;
            SiteTextBox.Text = login.Site;
            UserNameTextBox.Text = login.UserName;
            try
            {
                PasswordTextBox.Text = await service.DecryptAsync(login.Password);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var login = GetLoginFromTextBoxes();
            try
            {
                if (SaveAlreadyEncryptedCheckBox.IsChecked == false)
                {
                    login.Password = await service.EncryptAsync(PasswordTextBox.Text);
                }
                repo.Save(login);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private Login GetLoginFromTextBoxes()
        {
            return new Login
            {
                Site = SiteTextBox.Text,
                UserName = UserNameTextBox.Text,
                Password = PasswordTextBox.Text
            };
        }

        private void ClearDataInputs()
        {
            SiteTextBox.Clear();
            UserNameTextBox.Clear();
            PasswordTextBox.Clear();
            errorLabel.Content = string.Empty;
        }

        private void SetSearchText()
        {
            FindSiteTextBox.Text = (string)SiteListBox.SelectedItem;
        }

        private void PopulateListBox()
        {
            foreach (var l in logins)
            {
                SiteListBox.Items.Add(l.Site);
            }
        }

        private async Task RefreshList()
        {
            SearchingIsReady(false);
            SiteListBox.Items.Clear();
            await Task.Run(() =>
            {
                GetAllRecords();
            });
            SearchingIsReady(true);
            PopulateListBox();
        }

        #endregion

        #region UI events, Click events
        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
            InitializeButtonsState();
        }
        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            await Decrypt(encryptedTextBox.Text);
            InitializeButtonsState();
        }

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
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
            await FindSite(FindSiteTextBox.Text);
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FindSiteTextBox.Clear();
        }

        private void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDataInputs();
        }

        private void SaveAlreadyEncryptedCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAlreadyEncryptedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IService service = new EncryptionService();
                if (SaveAlreadyEncryptedCheckBox.IsChecked == true)
                {

                    PasswordTextBox.Text = service.Encrypt(PasswordTextBox.Text);
                }
                else
                {
                    PasswordTextBox.Text = service.Decrypt(PasswordTextBox.Text);
                }
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }

        }

        private void SiteListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SetSearchText();
        }

        private async void SiteListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetSearchText();
            await FindSite(FindSiteTextBox.Text);
        }

        private async void GetRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshList();
        }
        #endregion
    }
}
