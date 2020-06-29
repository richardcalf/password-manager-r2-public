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
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeButtonsState();
            Timer timer = new Timer();
            timer.Interval = 550;
            timer.Start();
            timer.Elapsed += OnTimedEvent;
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


        bool visible = false;
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (visible)
            {
                visible = false;
                Dispatcher.Invoke(() =>
                {
                    errorLabel.Visibility = Visibility.Hidden;
                });
            }
            else
            {
                visible = true;
                Dispatcher.Invoke(() =>
                {
                    errorLabel.Visibility = Visibility.Visible;
                });
            }
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
            IEnumerable<Login> logins = repo.GetLogins();
            if(SiteListBox.Items.Count.Equals(0))
            foreach(var l in logins)
            {
                
                SiteListBox.Items.Add(l.Site);
            }
            Login login = repo.GetLogins().Where(l => l.Site.StartsWith(FindSiteTextBox.Text)).FirstOrDefault();
            if (login != null)
            {
                SaveAlreadyEncryptedCheckBox.IsChecked = false;
                IServiceAsync service = new EncryptionService();
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
            else
            {
                SiteNotFoundLables();
            }
                    
        }
        
        private void SiteNotFoundLables()
        {
            SiteNotfoundlabel3.Foreground = Brushes.Red;
            SiteNotfoundlabel3.Content = "Site not found";
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            XmlPersistence repo = new XmlPersistence();
            Login login = GetLoginFromTextBoxes();
            try
            {
                if (SaveAlreadyEncryptedCheckBox.IsChecked == false)
                {
                    IServiceAsync service = new EncryptionService();
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

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FindSiteTextBox.Clear();
            SiteListBox.Items.Clear();
        }

        private void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDataInputs();
        }

        private void ClearDataInputs()
        {
            SiteTextBox.Clear();
            UserNameTextBox.Clear();
            PasswordTextBox.Clear();
            errorLabel.Content = string.Empty;
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
    }
}
