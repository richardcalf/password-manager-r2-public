using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using password.model;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private IRepository repo;
        private bool userExists;

        public App()
        {
            //do we have admin.admin? if not, we need to get the user to register.
            repo = new DatabasePersistence();
            var login = repo.GetLogin("admin.admin");
            userExists = login != null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!userExists)
            {
                RegistrationWindow regWindow = new RegistrationWindow();
                regWindow.ShowDialog();
            }
            else
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
            }
        }
    }
}
