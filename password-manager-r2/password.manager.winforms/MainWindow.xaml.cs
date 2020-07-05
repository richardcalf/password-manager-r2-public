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
using System.IO;
using System.Xml.Linq;
using password.resalter;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IEnumerable<Login> logins;
        IRepository repo = new XmlPersistence();
        IServiceAsync service;
        IResalterAsync resalter;
        private const string updateSucceeded = "Update Succeeded";
        private const string updateFailed = "Update Failed";
        private string globalSalt;
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeButtonsState();
            GetAllRecordsAsync();
            globalSalt = Settings.GetValueFromSettingKey("salt");
            CurrentSaltTextBox.Text = globalSalt;
            service = GetEncryptionService();
            FilePathTextBox.Text = Settings.GetValueFromSettingKey("push");
            RevertPathButton.Content = @"<< Revert";
            PullButton.Content = @"<< Pull Logins";
            CurrentSaltTextBox.IsEnabled = false;
            SaveSaltButton.IsEnabled = false;
            AdvancedCanvas.Visibility = Visibility.Hidden;
        }

        private EncryptionService GetEncryptionService()
        {
            if (globalSalt == null)
            {
                return new EncryptionService();
            }
            else
            {
                return new EncryptionService(globalSalt);
            }
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

        private void PushLogins()
        {
            var pushfile = Settings.GetValueFromSettingKey("push");

            if (pushfile == null)
            {
                FailedUIMessage("push configuration is not setup");
                return;
            }

            if (File.Exists("Logins.xml"))
            {
                File.Delete(pushfile);
                File.Copy("Logins.xml", pushfile);
                SuccessUIMessage("File has been pushed");
            }
            else
            {
                FailedUIMessage("No file to push");
            }
        }

        private void PullLogins()
        {
            var pullfile = Settings.GetValueFromSettingKey("push");

            if (pullfile == null)
            {
                FailedUIMessage("push configuration is not setup");
                return;
            }

            if (File.Exists(pullfile))
            {
                File.Delete("Logins.xml");
                File.Copy(pullfile, "Logins.xml" );
                SuccessUIMessage("File has been pulled");
            }
            else
            {
                FailedUIMessage("No file to pull");
            }
        }

        private void SaveSetting(string key, string value)
        {
            Settings.SaveAppSetting(key, value);
            SuccessUIMessage($"Setting [{key}] has been updated");
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

        private void SuccessUIMessage(string message)
        {
            UpdateLabel.Foreground = Brushes.Green;
            UpdateLabel.Content = message;
        }

        private void FailedUIMessage(string message)
        {
            UpdateLabel.Foreground = Brushes.Red;
            UpdateLabel.Content = message;
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
                SuccessUIMessage(updateSucceeded);
            }
            catch (Exception ex)
            {
                FailedUIMessage(updateFailed);
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
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you want to delete {SiteTextBox.Text}?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                bool deleted = DeleteLogin(SiteTextBox.Text);
                if (deleted)
                {
                    RemoveItemFromListBox();
                    ClearDataInputs();
                    SuccessUIMessage("Delete Succeeded");
                }
                else
                {
                    FailedUIMessage("Delete Failed");
                }
            }
        }

        private void RemoveItemFromListBox()
        {
            foreach (var item in SiteListBox.Items)
            {
                if (item.ToString().Equals(SiteTextBox.Text))
                {
                    int index = SiteListBox.Items.IndexOf(SiteTextBox.Text);
                    SiteListBox.Items.RemoveAt(index);
                    break;
                }
            }
        }

        private void ClearLitButton_Click(object sender, RoutedEventArgs e)
        {
            SiteListBox.Items.Clear();
            ClearUpdateUIMessage();
        }
        

        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to push the Logins to [{Settings.GetValueFromSettingKey("push")}] ?", "Push Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                PushLogins();
            }
        }

        private void SaveSettingButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you want Save this Push File Path [{FilePathTextBox.Text}] ?", "Push File Path Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveSetting("push", FilePathTextBox.Text);
            }
        }

        private void RevertPathButton_Click(object sender, RoutedEventArgs e)
        {
            FilePathTextBox.Text = Settings.GetValueFromSettingKey("push");
        }

        private void PullButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to pull the Logins from [{Settings.GetValueFromSettingKey("push")}] ?", "Pull File Path Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                PullLogins();
            }
        }

        private void SaveSaltButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSetting("salt", CurrentSaltTextBox.Text);
            ReSaltTextBox.Text = string.Empty;
            ReSaltEasterEgg();
            service = new EncryptionService(CurrentSaltTextBox.Text);
        }

        private async void ReSaltButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you would like to Re-Salt?", "Re-Salt Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(ReSaltTextBox.Text))
                {
                    FailedUIMessage("Unable to Re-Salt with an empty string");
                    return;
                }
                if (ReSaltTextBox.Text == CurrentSaltTextBox.Text)
                {
                    FailedUIMessage("Salt has not changed");
                    return;
                }
                await ResaltAllLogins(CurrentSaltTextBox.Text, ReSaltTextBox.Text);
                CurrentSaltTextBox.Text = ReSaltTextBox.Text;
            }
        }

        private async Task ResaltAllLogins(string previousSalt, string newSalt)
        {
            resalter = new Resalter();
            logins = repo.GetLogins();

            var newSaltedLogins = await resalter.ResaltAsync(previousSalt, newSalt, logins);

            await Task.Run(() =>
            {
                repo.Save(newSaltedLogins);
            });

            logins = newSaltedLogins;
            SaveSetting("salt", newSalt);
            SuccessUIMessage("Re-Salting Succeeded");
            
            service = new EncryptionService(newSalt);
        }

        private void ReSaltTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ReSaltEasterEgg();
        }

        private void ReSaltEasterEgg()
        {
            var enabled = ReSaltTextBox.Text.Equals("reverb");
            CurrentSaltTextBox.IsEnabled = enabled;
            SaveSaltButton.IsEnabled = enabled;
            ReSaltButton.IsEnabled = !enabled;
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleAdvancedPanel();
        }

        private void ToggleAdvancedPanel()
        {
            if (AdvancedCanvas.Visibility == Visibility.Hidden)
            {
                AdvancedCanvas.Visibility = Visibility.Visible;
                AdvancedButton.Content = "Advanced <<";
            }
            else
            {
                AdvancedCanvas.Visibility = Visibility.Hidden;
                AdvancedButton.Content = "Advanced >>";
            }
        }
        #endregion
    }
}
