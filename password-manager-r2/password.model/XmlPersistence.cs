using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace password.model
{
    public class XmlPersistence : IRepository
    {
        public void Save(Login model)
        {
            PersistToXml(model);
        }

        public Login GetLogin(string site)
        {
            return GetLoginList().FirstOrDefault(l => l.Site.StartsWith(site));
        }

        public IEnumerable<Login> GetLogins()
        {
            return GetLoginList();
        }

        public bool IsValid(Login model)
        {
            return !(string.IsNullOrWhiteSpace(model.Site) ||
                string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.Password));
        }

        private IEnumerable<Login> GetLoginList()
        {
            if (File.Exists("Logins.xml"))
            {
                XElement doc = XElement.Load("Logins.xml");

                return
                    from login in doc.Elements("Login")
                    orderby login.Element("Site").Value
                    select new Login
                    {
                        Site = login.Element("Site").Value,
                        UserName = login.Element("UserName").Value,
                        Password = login.Element("Password").Value
                    };
            }
            return new List<Login>();
        }

        private bool AmendRecord(Login model)
        {
            var doc = XDocument.Load("Logins.xml");

            XElement login =
                  (from lgin in doc.Descendants("Login")
                   where lgin.Element("Site").Value == model.Site
                   select lgin).SingleOrDefault();

            if (login == null) return false;

            login.Element("UserName").Value = model.UserName;
            login.Element("Password").Value = model.Password;
            doc.Save("Logins.xml");

            return true;
        }

        private void PersistToXml(Login model)
        {
            if (!IsValid(model)) throw new Exception("Fill out all fields.");

            if (!File.Exists("Logins.xml"))
            {
                AddNewXmlFile(model);
            }
            else
            {
                if (!AmendRecord(model))
                {
                    AddNewRecord(model);
                }
            }
        }

        private void AddNewRecord(Login model)
        {
            var doc = XDocument.Load("Logins.xml");

            var login = GetLoginElement(model);

            doc.Root.Add(login);
            doc.Save("Logins.xml");
        }

        private void AddNewXmlFile(Login model)
        {
            var doc = new XDocument();
            var root = new XElement("Logins");

            var login = GetLoginElement(model);

            root.Add(login);

            doc.Add(root);
            doc.Save("Logins.xml");
        }

        private XElement GetLoginElement(Login model)
        {
            return new XElement("Login",
                                 new XElement("Site", model.Site),
                                 new XElement("UserName", model.UserName),
                                 new XElement("Password", model.Password));
        }
    }
}
