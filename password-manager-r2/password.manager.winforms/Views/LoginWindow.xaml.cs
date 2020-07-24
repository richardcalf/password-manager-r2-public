using password.model;
using System;
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
using password.settings;
using password.service;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isAuthenticated;
        private readonly IPasswordManagerLoginService applicationLoginService;
        private string salt;
        private IServiceAsync service;

        //to set username for convenience
        private IRepository repo;
        public LoginWindow()
        {
            salt = Settings.GetValueFromSettingKey("salt");
            service = EncryptionServiceFactory.GetEncryptionServiceAsync(salt);
            isAuthenticated = false;
            applicationLoginService = new PasswordManagerLoginService();
            repo = new DatabasePersistence();
            InitializeComponent(); 
            SetUIUsername();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Login Form is closing");
            if(isAuthenticated)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            isAuthenticated = false;
            Close();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await Login();
        }

        private async Task Login()
        {
            var login = GetLoginFromUI();
            login.Password = await service.EncryptAsync(login.Password);
            isAuthenticated = applicationLoginService.Login(login);

            if (!isAuthenticated)
            {
                InvalidLoginUIMessage();
            }
            else
            {
                Close();
            }
        }

        private void InvalidLoginUIMessage()
        {
            InvalidLoginLabel.Foreground = Brushes.Red;
            InvalidLoginLabel.Content = "Invalid login details";
        }

        private Login GetLoginFromUI()
        {
            var login = new Login();

            login.UserName = UserNameTextBox.Text;
            login.Password = PasswordTextBox.Password;
            return login;
        }

        private void SetUIUsername()
        {
            var login = repo.GetLogin("admin.admin");
            if (login != null)
            {
                UserNameTextBox.Text = login.UserName;
                PasswordTextBox.Focus();
            }
        }

        private void UserNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            InvalidLoginLabel.Content = string.Empty;
        }

        private void PasswordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            InvalidLoginLabel.Content = string.Empty;
        }
    }
}
