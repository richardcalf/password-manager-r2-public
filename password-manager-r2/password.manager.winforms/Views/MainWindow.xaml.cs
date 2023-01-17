﻿using password.manager.winforms.Views.Themes;
using password.model;
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
using CliWrap;
using CliWrap.Buffered;
using System.Threading;
using CliWrap.EventStream;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.Design;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string loginFilePath;
        private readonly string gitRepoPath;
        private const string updateSucceeded = "Update Succeeded";
        private const string updateFailed = "Update Failed";
        private IUIBroker broker;
        public MainWindow(IUIBroker broker)
        {
            InitializeComponent();
            InitializeData();
            InitializeThemeComboBox();
            this.broker = broker;
            loginFilePath = Settings.GetValueFromSettingKey("loginFilePath");
            gitRepoPath = loginFilePath.Replace(@"\Logins.xml", "");

            if (Settings.GetValueFromSettingKey("GitIntegration") == "yes")
            {
                broker.DataUpdate += UpdateGitHub;
                _ = StartUpPull();
            }
            broker.SettingSaved += SettingSaved;
            broker.Resalted += ResaltingDone;
            broker.DataReady += DataInputIsReady;

            CurrentSaltTextBox.Text = broker.Salt;
            CurrentSaltTextBox.IsEnabled = false;
            AdvancedCanvas.Visibility = Visibility.Hidden;
            
            siteListFilterTextBox.Focus();
            ApplyTheme();
            _ = this.broker.GetAllRecordsAsync();
        }

        #region Update GitHub
        private async Task StartUpPull()
        {
            try
            {
                await InvokeGit(new[] { "pull" });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    FailedUIMessage($"GitHub failed: {ex.Message}");
                });
            }
        }
        private async Task<int> InvokeGit(IEnumerable<string> commands)
        {
            var cmd = Cli.Wrap("git")
                .WithArguments(commands)
                .WithWorkingDirectory(workingDirPath: gitRepoPath);

            int exitCode = 255;

            await foreach (var cmdEvt in cmd.ListenAsync())
            {
                switch (cmdEvt)
                {
                    case ExitedCommandEvent exited:
                        {
                            exitCode = exited.ExitCode;
                            break;
                        }
                }
            }
            return exitCode;
        }

        private async void UpdateGitHub(string updateMessage)
        {
            Dispatcher.Invoke(() => { DataInputIsReady(false) ; });
            try
            {
                int pull = await InvokeGit(new[] { "pull" });
                if (pull == 0)
                {
                    int add = await InvokeGit(new[] { "add", "Logins.xml" });
                    if (add == 0)
                    {
                        int commit = await InvokeGit(new[] { "commit", "-m", $"{updateMessage}" });
                        if (commit == 0)
                        {
                            int push = await InvokeGit(new[] { "push" });
                            if (push == 0)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    SuccessPushMessage("Data pushed to GitHub");
                                    DataInputIsReady(true);
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                Dispatcher.Invoke(() =>
                {
                    //will need to log exception message 
                    FailedUIMessage($"git failed: {ex.Message}");
                    DataInputIsReady(true);
                });
            }
            
        }
        #endregion

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

        private void DataInputIsReady(bool ready)
        {
            FindSiteButton.IsEnabled = ready;
            GetRecordsButton.IsEnabled = ready;
            UpdateButton.IsEnabled = ready;
            DeleteButton.IsEnabled = ready;
            ReSaltButton.IsEnabled = ready;
            SiteListBox.IsEnabled = ready;
            SelectSiteButton.IsEnabled = ready;
            ReSaltButton.IsEnabled = ready;
            ReSaltTextBox.IsEnabled = ready;
            siteListFilterTextBox.IsEnabled = ready;
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

        private void SuccessPushMessage(string message)
        {
            UpdateLabel.Foreground = Brushes.Blue;
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
            var login = broker.GetLogin(site);
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
                    broker.Save(login);
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
                return broker.Delete(site);
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
                if (SiteListBox.HasItems)
                {
                    FindSiteTextBox.Text = (string)SiteListBox.Items[0];
                }
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
            if(!string.IsNullOrWhiteSpace(input))
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
            siteListFilterTextBox.Clear();
            SiteListBox.Items.Clear();
            CountRecordslabel.Content = string.Empty;
            FindSiteTextBox.Clear();
            ClearDataInputs();
            ClearUpdateUIMessage();
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
            }
        }

        private void ResaltingDone(string resalt)
        {
            CurrentSaltTextBox.Text = resalt;
            ReSaltTextBox.Text = string.Empty;
            SuccessUIMessage("Re-Salting Succeeded");
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleAdvancedPanel();
        }

        private void visualModeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ApplyTheme();
            SaveTheme();
        }

        private async void FindSiteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await FindSite(FindSiteTextBox.Text);
            }
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
                if (SiteListBox.SelectedItem != null)
                {
                    Delete(SiteListBox.SelectedItem.ToString());
                    return;
                }
                if (SiteListBox.HasItems)
                {
                    //if the execution reaches here it means that
                    //there is only one record on the grid and using keyboard only
                    //the damn top record never becomes selected. feels like a ui control bug.
                    Delete(SiteListBox.Items[0].ToString());
                }
            }
            else
            {
                EnterKeySetSearchFindSite(e);
            }
        }

        private void CopyOnFocus(object sender)
        {
            if (sender is TextBox)
            {
                var textBox = sender as TextBox;
                Clipboard.SetText(textBox.Text);
            }
        }

        private void FindSiteTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CopyOnFocus(sender);
        }

        private void SiteTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CopyOnFocus(sender);
        }

        private void UserNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CopyOnFocus(sender);
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CopyOnFocus(sender);
        }
        #endregion

        private void PasswordTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PasswordTextBox.Text = broker.GenerateRndPasswrd();
            CopyOnFocus(sender);
        }
    }
}
