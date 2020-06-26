using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace password.model
{
    public class XmlPersistence : IRepository
    {
        public Login GetLogin(string site)
        {
            Login model = new Login();

            return model;
        }

        public int Save(Login model)
        {
            var document = new XDocument();
            var logins = new XElement("Logins");
            //////
            var login = new XElement("Login");
            var site = new XElement("Site", model.Site);
            var username = new XElement("UserName", model.UserName);
            var password = new XElement("Password", model.Password);
            login.Add(site);
            login.Add(username);
            login.Add(password);
            logins.Add(login);

            document.Add(logins);
            document.Save("Logins.xml");

            return 0;
        }
    }
}
