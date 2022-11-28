using password.manager.winforms.Views.Themes;
using password.settings;
using password.uibroker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;



namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string loginFilePath;
        private const string updateSucceeded = "Update Succeeded";
        private const string updateFailed = "Update Failed";
        private IUIBroker broker;
        public MainWindow(IUIBroker broker)
        {
            InitializeComponent();
            InitializeData();
            InitializeThemeComboBox();
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
            loginFilePath = Settings.GetValueFromSettingKey("loginFilePath");
            siteListFilterTextBox.Focus();

            ApplyTheme();
            _ = this.broker.GetAllRecordsAsync();
        }

        #region private UI coupled methods
        private void InitializeThemeComboBox()
        {
            foreach (var theme in ThemeHelper.GetThemeList())
            {
                visualModeComboBox.Items.Add(theme);
            }

            int tIndex = 0;
            int.TryParse(ThemeHelper.GetThemeSetting(), out tIndex);
            visualModeComboBox.SelectedIndex = tIndex;
        }

        private void ApplyTheme()
        {
            ThemeHelper.ApplyTheme(theGrid, visualModeComboBox.SelectedIndex);
        }

        private void SaveTheme()
        {
            ThemeHelper.SaveThemeSetting(visualModeComboBox.SelectedIndex);
        }

        private void PushLogins()
        {
            var pushfile = broker.PushPath;

            if (pushfile == null)
            {
                FailedUIMessage("push configuration is not setup");
                return;
            }

            if (File.Exists(loginFilePath))
            {
                File.Delete(pushfile);
                File.Copy(loginFilePath, pushfile);
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
                File.Delete(loginFilePath);
                File.Copy(pullfile, loginFilePath);
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

        private void DebugUIMessage(string message)
        {
            UpdateLabel.Foreground = Brushes.Blue;
            UpdateLabel.Content = message;
        }

        private void PrintException(string message)
        {
            errorLabel.Foreground = Brushes.Red;
            errorLabel.Content = message;
        }

        private void InitializeData()
        {
            errorLabel.Content = string.Empty;
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
            if (login == null) { SiteTextBox.Text = ""; return; }
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
            if (!broker.Repo.IsValid(model))
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
            if (SiteListBox.SelectedItems.Count == 0)
            {
                //if (SiteListBox.Items.Count > 0)
                //    FindSiteTextBox.Text = (string)SiteListBox.Items[0];
                FindSiteTextBox.Text = "";
            }
            else
            {
                FindSiteTextBox.Text = (string)SiteListBox.SelectedItem;
            }
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

        private async Task PopulateListBoxFiltered(string input)
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var l in broker.Logins
                                            .Where(l => l.Site.ToLower()
                                            .Contains(input.ToLower())))
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
            siteListFilterTextBox.Clear();
            await broker.GetAllRecordsAsync();
            ClearUpdateUIMessage();
            await PopulateListBox();
            UpdateRecordCountLabel(broker.Logins.Count());
            UpdateRecordCountLabel(SiteListBox.Items.Count);
        }

        private async Task FilterList()
        {
            SiteListBox.Items.Clear();
            await broker.GetAllRecordsAsync();
            ClearUpdateUIMessage();
            await PopulateListBoxFiltered(siteListFilterTextBox.Text);
            ClearUpdateUIMessage();
            UpdateRecordCountLabel(SiteListBox.Items.Count);
        }

        private void RemoveItemFromListBox(string site)
        {
            foreach (var item in SiteListBox.Items)
            {
                if (item.ToString().Equals(site))
                {
                    int index = SiteListBox.Items.IndexOf(site);
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
            ClearUpdateUIMessage();
        }
        private async void decryptButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void encryptButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void plainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void encryptedTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void decryptedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void decryptedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
           
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

        private void button_Click(object sender, EventArgs e)
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

        private async void sitListFilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key != Key.Tab) && (e.Key != Key.Enter) && (e.Key != Key.LeftShift))
            {
                await FilterList();
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateLoginFromUI();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete(SiteTextBox.Text);
        }

        private void Delete(string site)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you want to delete {site}?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                bool deleted = DeleteLogin(site);
                if (deleted)
                {
                    RemoveItemFromListBox(site);
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
            FindSiteTextBox.Clear();
            ClearDataInputs();
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

        private void RandomPwGenButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = broker.GenerateRndPasswrd();
        }

        private void visualModeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ApplyTheme();
            SaveTheme();
        }

        private void cpyUsrNameBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(UserNameTextBox.Text);
        }

        private void cpyPsswrdBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(PasswordTextBox.Text);
        }
        #endregion

        private async void FindSiteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await FindSite(FindSiteTextBox.Text);
            }
        }

        private async void sitListFilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySetSearchFindSite(e);
        }

        private async void EnterKeySetSearchFindSite(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetSearchText();
                await FindSite(FindSiteTextBox.Text);
            }
        }

        private void SiteListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Delete(SiteListBox.SelectedItem.ToString());
            }
            else
            {
                EnterKeySetSearchFindSite(e);
            }
        }
    }
}
