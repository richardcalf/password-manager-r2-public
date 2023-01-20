
using password.git.integration;
using password.manager.winforms.Views.Themes;
using password.model;
using password.settings;
using password.uibroker;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Logging;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IUIBroker broker;
        private bool isAuthenticated;
        private readonly IPasswordManagerLoginService applicationLoginService;
        
        public LoginWindow(IUIBroker broker, IPasswordManagerLoginService pwManager)
        {
            this.broker = broker;
            isAuthenticated = false;
            applicationLoginService = pwManager;
            InitializeComponent(); 
            SetUIUsername();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            int tIndex = 0;
            int.TryParse(ThemeHelper.GetThemeSetting(), out tIndex);
            ThemeHelper.ApplyTheme(loginGrid, tIndex);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Login Form is closing");
            if(isAuthenticated)
            {
                MainWindow mainWindow = new MainWindow(this.broker);
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
            login.Password = await broker.EncryptionService.EncryptAsync(login.Password);
            isAuthenticated = applicationLoginService.Login(login);

            if (!isAuthenticated)
            {
                InvalidLoginUIMessage();
            }
            else
            {
                CloseWithPull();   
            }
        }

        private async void CloseWithPull()
        {
            if (Settings.GetValueFromSettingKey("GitIntegration") == "yes")
            {
                this.Visibility = Visibility.Hidden;
                var gitRepoPath = Settings.GetValueFromSettingKey("repoPath");
                var pull = await GitIntegration.InvokeGit(new[] { "pull" }, gitRepoPath);
                if (pull == 0)
                {
                    Close();
                }
                else
                    Environment.Exit(0);
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
            var login = broker.Repo.GetLogin("admin.admin");
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
