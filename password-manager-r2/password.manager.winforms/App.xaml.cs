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

        private UIBroker broker;
        private bool userExists;

        public App()
        {
            broker = new UIBroker();
            //do we have admin.admin? if not, we need to get the user to register.
            var login = broker.Repo.GetLogin("admin.admin");
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
