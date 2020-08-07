using Ninject;
using password.manager.winforms;
using password.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using password.resalter;

namespace password.xml.bootstrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IKernel Container = new StandardKernel();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            Container.Bind<IRepository>().To<XmlPersistence>();
            Container.Bind<ILoginService>().To<XmlPersistence>();
            Container.Bind<IResalterAsync>().To<Resalter>();
            Container.Bind<IUIBroker>().To<UIBroker>();
            Container.Bind<IPasswordManagerLoginService>().To<PasswordManagerLoginService>();
            
        }

        private void ComposeObjects()
        {
            var repository = Container.Get<IRepository>();
            var login = repository.GetLogin("admin.admin");
            var userExists = login != null;
            if(!userExists)
            {
                Current.MainWindow = Container.Get<RegistrationWindow>();
            }
            else
            {
                Current.MainWindow = Container.Get<LoginWindow>();
            }
        }

        
    }
}
