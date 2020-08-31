using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Ajax.Utilities;
using password.model;
using password.resalter;
using password.uibroker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace password.mvc
{
    public class ContainerConfig
    { 
        internal static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<XmlPersistence>().As<IRepository, ILoginService>().InstancePerRequest();
            builder.RegisterType<Resalter>().As<IResalterAsync>().InstancePerRequest();
            builder.RegisterType<UIBroker>().As<IUIBroker>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}