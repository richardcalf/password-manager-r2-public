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

        private async Task Encrypt(string value)
        {
            try
            {
                encryptedTextBox.Text = await service.EncryptAsync(value);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private void ClearUpdateUIMessage()
        {
            UpdateLabel.Content = string.Empty;
        }

        private void UpdateSuccessUIMessage()
        {
            UpdateLabel.Foreground = Brushes.Green;
            UpdateLabel.Content = "Update Succeeded";
        }

        private void DeleteSuccessUIMessage()
        {
            UpdateLabel.Foreground = Brushes.Green;
            UpdateLabel.Content = "Delete Succeeded";
        }

        private void DeleteFailedUIMessage()
        {
            UpdateLabel.Foreground = Brushes.Red;
            UpdateLabel.Content = "Delete Failed";
        }


        private void UpdateFailedUIMessage()
        {
            UpdateLabel.Foreground = Brushes.Red;
            UpdateLabel.Content = "Update Failed";
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
            var login = repo.GetLogin(site);
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

        private async Task UpdateLogin()
        {
            var login = GetLoginFromTextBoxes();
            try
            {
                login.Password = await service.EncryptAsync(PasswordTextBox.Text);

                await Task.Run(() =>
                {
                    repo.Save(login);
                });
                UpdateSuccessUIMessage();
            }
            catch (Exception ex)
            {
                UpdateFailedUIMessage();
                PrintException(ex.Message);
            }
        }

        private bool DeleteLogin(string site)
        {
            try
            {
                return repo.Delete(site);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
                return false;
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
            ClearUpdateUIMessage();
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
            ClearUpdateUIMessage();
            SearchingIsReady(true);
            PopulateListBox();
        }

        #endregion

        #region UI events, Click events
        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
            InitializeButtonsState();
            ClearUpdateUIMessage();
        }
        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            await Decrypt(encryptedTextBox.Text);
            InitializeButtonsState();
            ClearUpdateUIMessage();
        }

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            await Encrypt(plainTextBox.Text);
            InitializeButtonsState();
            ClearUpdateUIMessage();
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
            ClearUpdateUIMessage();
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

        }

        private void SiteListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SetSearchText();
            ClearUpdateUIMessage();
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

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateLogin();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to delete {SiteTextBox.Text}?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                bool deleted = DeleteLogin(SiteTextBox.Text);
                if (deleted)
                {
                    RemoveItemFromListBox();
                    ClearDataInputs();
                    DeleteSuccessUIMessage();
                }
                else
                {
                    DeleteFailedUIMessage();
                }
            }
        }

        private void RemoveItemFromListBox()
        {
            bool listBoxHasDeletedSite = false;
            foreach (var item in SiteListBox.Items)
            {
                listBoxHasDeletedSite = item.ToString().Equals(SiteTextBox.Text);
                if (listBoxHasDeletedSite)
                {
                    break;
                }
            }
            if (listBoxHasDeletedSite)
            {
                int index = SiteListBox.Items.IndexOf(SiteTextBox.Text);
                SiteListBox.Items.RemoveAt(index);
            }
        }

        private void ClearLitButton_Click(object sender, RoutedEventArgs e)
        {
            SiteListBox.Items.Clear();
            ClearUpdateUIMessage();
        }
        #endregion
    }
}
