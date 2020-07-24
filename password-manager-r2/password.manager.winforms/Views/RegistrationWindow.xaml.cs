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
using password.model;
using password.service;
using password.settings;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private bool isAuthenticated;
        private IRepository repo;
        private IPasswordManagerLoginService applicationLoginService;
        private IServiceAsync service;

        public RegistrationWindow()
        {
            repo = new DatabasePersistence();
            applicationLoginService = new PasswordManagerLoginService();
            Settings.RemoveAppSettings(new List<string> { "salt", "push" });
            service = EncryptionServiceFactory.GetEncryptionServiceAsync(null);
            InitializeComponent();
            UserNameTextBox.Focus();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateRegistration())
            {
                var login = GetLoginFromUI();
                login.Password = await service.EncryptAsync(login.Password);
                var registered = applicationLoginService.Register(login);
                isAuthenticated = registered;
                Close();
            }
        }

        private bool ValidateRegistration()
        {
            if (UserExists())
            {
                InvalidRegistrationUIMessage();
                return false;
            }

            if (UserNameTextBox.Text.Length < 5)
            {
                InvalidRegistrationUIMessage("Username must contain at least 5 characters");
                return false;
            }
            if (PasswordTextBox.Password.Length < 5)
            {
                InvalidRegistrationUIMessage("Password must contain at least 5 characters");
                return false;
            }
            if (PasswordTextBox.Password != ConfirmPasswordBox.Password)
            {
                InvalidRegistrationUIMessage("Passwords do not match");
                return false;
            }
            return true;
        }

        private Login GetLoginFromUI()
        {
            Login login = new Login();
            login.UserName = UserNameTextBox.Text;
            login.Password = PasswordTextBox.Password;

            return login;
        }

        #region private methods
        private void InvalidRegistrationUIMessage()
        {
            InvalidRegistrationLabel.Foreground = Brushes.Red;
            InvalidRegistrationLabel.Content = "Application login already exists";
        }

        private void InvalidRegistrationUIMessage(string message)
        {
            InvalidRegistrationLabel.Foreground = Brushes.Red;
            InvalidRegistrationLabel.Content = message;
        }

        private bool UserExists()
        {
            var login = repo.GetLogin("admin.admin");
            return login != null;
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Login Form is closing");
            if (isAuthenticated)
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
    }
}
