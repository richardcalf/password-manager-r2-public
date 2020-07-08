using password.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace password.login.service
{
    public class XmlLoginService : ILoginService
    {

        private IRepository repo;

        public XmlLoginService()
        {
            repo = new XmlPersistence();
        }


        #region ILoginService
        public bool Login(Login login)
        {
            return LoginExists(login);
        }

        public bool Register(Login login)
        {
            if (File.Exists("Logins.xml"))
            {
                File.Delete("Logins.xml");
            }
            repo.Save(login);
            return true;
        }
        #endregion

        #region private methods
        private bool LoginExists(Login model)
        {
            var doc = XDocument.Load("Logins.xml");
            XElement login =
                (from lgin in doc.Descendants("Login")
                 where lgin.Element("Site").Value == model.Site &
                 lgin.Element("UserName").Value == model.UserName &
                 lgin.Element("Password").Value == model.Password
                 select lgin).SingleOrDefault();
            return login != null;
        }
        #endregion
    }
}
