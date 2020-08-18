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
        private const string updateSucceeded = "Update Succeeded";
        private const string updateFailed = "Update Failed";
        private IUIBroker broker;
        public MainWindow(IUIBroker broker)
        {
            InitializeComponent();
            InitializeData();
            InitializeButtonsState();
            this.broker = broker;
            broker.SettingSaved += SettingSaved;
            broker.Resalted += ResaltingDone;
            broker.DataReady += DataInputIsReady;
            

            CurrentSaltTextBox.Text = broker.Salt;
            FilePathTextBox.Text = broker.PushPath;
            RevertPathButton.Content = @"<< Revert";
            PullButton.Content = @"<< Pull Logins";
            CurrentSaltTextBox.IsEnabled = false;
            SaveSaltButton.IsEnabled = false;
            AdvancedCanvas.Visibility = Visibility.Hidden;
            _ = this.broker.GetAllRecordsAsync();
        }

        #region private UI coupled methods
        

        private void PushLogins()
        {
            var pushfile = broker.PushPath;

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
            var pullfile = broker.PushPath;

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

        private void DataInputIsReady(bool ready)
        {
            FindSiteButton.IsEnabled = ready;
            GetRecordsButton.IsEnabled = ready;
            UpdateButton.IsEnabled = ready;
            DeleteButton.IsEnabled = ready;
            PushButton.IsEnabled = ready;
            PullButton.IsEnabled = ready;
            ReSaltButton.IsEnabled = ready;
            SiteListBox.IsEnabled = ready;
            SelectSiteButton.IsEnabled = ready;
            ReSaltButton.IsEnabled = ready;
            ReSaltTextBox.IsEnabled = ready;
            RandomPwGenButton.IsEnabled = ready;
        }

        private void ClearUpdateUIMessage()
        {
            UpdateLabel.Content = string.Empty;
        }

        private void SuccessUIMessage(string message)
        {
            UpdateLabel.Foreground = Brushes.Green;
            UpdateLabel.Content = message;
            errorLabel.Content = string.Empty;
        }

        private void SettingSaved(string key)
        {
            UpdateLabel.Foreground = Brushes.Green;
            UpdateLabel.Content = $"Setting ({key}) has been saved";
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
            var login = broker.Repo.GetLogin(site);
            await ShowLoginOnUI(login);
        }

        /// <summary>
        /// Show Login Model in TextBoxes
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private async Task ShowLoginOnUI(model.Login login)
        {
            ClearDataInputs();
            if (login == null) { SiteTextBox.Text = "Not found"; return; }
            SiteTextBox.Text = login.Site;
            UserNameTextBox.Text = login.UserName;
            try
            {
                PasswordTextBox.Text = await broker.DecryptAsync(login.Password);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private async Task UpdateLoginFromUI()
        {
            var login = GetLoginFromTextBoxes();
            try
            {
                ValidateLogin(login);
                login.Password = await broker.EncryptAsync(login.Password);

                await Task.Run(() =>
                {
                    broker.Repo.Save(login);
                });
                SuccessUIMessage(updateSucceeded);
            }
            catch (Exception ex)
            {
                FailedUIMessage(updateFailed);
                PrintException(ex.Message);
            }
        }

        private void ValidateLogin(model.Login model)
        {
            if(!broker.Repo.IsValid(model))
            {
                throw new Exception("input data is not valid");
            }
        }

        private bool DeleteLogin(string site)
        {
            try
            {
                return broker.Repo.Delete(site);
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
                return false;
            }
        }

        private model.Login GetLoginFromTextBoxes()
        {
            return new model.Login
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

        private async Task PopulateListBox()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var l in broker.Logins)
                    {
                        SiteListBox.Items.Add(l.Site);
                    }
                });
            });
        }

        private void UpdateRecordCountLabel(int count)
        {
            CountRecordslabel.Content = $"record count: {count}";
        }

        private async Task RefreshList()
        {
            SiteListBox.Items.Clear();
            await broker.GetAllRecordsAsync();
            ClearUpdateUIMessage();
            await PopulateListBox();
            UpdateRecordCountLabel(broker.Logins.Count());
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
            UpdateRecordCountLabel(SiteListBox.Items.Count);
        }

        private void ReSaltEasterEgg()
        {
            var enabled = ReSaltTextBox.Text.Equals("resync");
            CurrentSaltTextBox.IsEnabled = enabled;
            SaveSaltButton.IsEnabled = enabled;
            ReSaltButton.IsEnabled = !enabled;
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

        #region UI events, Click events
        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
            InitializeButtonsState();
            ClearUpdateUIMessage();
        }
        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decryptedTextBox.Text = await broker.DecryptAsync(encryptedTextBox.Text);
                InitializeButtonsState();
                ClearUpdateUIMessage();
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
        }

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                encryptedTextBox.Text = await broker.EncryptAsync(plainTextBox.Text);
                InitializeButtonsState();
                ClearUpdateUIMessage();
            }
            catch (Exception ex)
            {
                PrintException(ex.Message);
            }
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
            await UpdateLoginFromUI();
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

        private void ClearLitButton_Click(object sender, RoutedEventArgs e)
        {
            SiteListBox.Items.Clear();
            CountRecordslabel.Content = string.Empty;
            ClearUpdateUIMessage();
        }

        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to push the Logins to [{broker.PushPath}] ?", "Push Confirmation", MessageBoxButton.YesNo);
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
                broker.SaveSetting("push", FilePathTextBox.Text);
            }
        }

        private void RevertPathButton_Click(object sender, RoutedEventArgs e)
        {
            FilePathTextBox.Text = broker.PushPath;
        }

        private void PullButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to pull the Logins from [{broker.PushPath}] ?", "Pull File Path Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                PullLogins();
            }
        }

        private void SaveSaltButton_Click(object sender, RoutedEventArgs e)
        {
            broker.SaveSetting("salt", CurrentSaltTextBox.Text);
            ReSaltTextBox.Text = string.Empty;
            errorLabel.Content = string.Empty;
            ReSaltEasterEgg();
            broker.RefreshEncryptionSalt();
        }

        private bool ResaltModalAndValidation()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you would like to Re-Salt?", "Re-Salt Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(ReSaltTextBox.Text))
                {
                    FailedUIMessage("Unable to Re-Salt with an empty string");
                    return false;
                }
                if (ReSaltTextBox.Text == CurrentSaltTextBox.Text)
                {
                    FailedUIMessage("Salt has not changed");
                    return false;
                }
                DataInputIsReady(false);
                ClearUpdateUIMessage();
                return true;
            }
            return false;
        }

        private async void ReSaltButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResaltModalAndValidation())
            {
                try
                {
                    await broker.ResaltAllLogins(CurrentSaltTextBox.Text, ReSaltTextBox.Text);
                }
                catch (Exception ex)
                {
                    FailedUIMessage("Re-Salting Failed");
                    PrintException(ex.Message);
                }
                finally
                {
                    DataInputIsReady(true);
                }
            }
        }

        private void ResaltingDone(string resalt)
        {
            CurrentSaltTextBox.Text = resalt;
            ReSaltTextBox.Text = string.Empty;
            SuccessUIMessage("Re-Salting Succeeded");
        }

        private void ReSaltTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            ReSaltEasterEgg();
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleAdvancedPanel();
        }
        #endregion

        private void RandomPwGenButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = broker.GenerateRndPasswrd();
        }
    }
}
