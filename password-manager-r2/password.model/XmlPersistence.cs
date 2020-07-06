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
            AmendRecord(model);
        }

        public void Save(IEnumerable<Login> models)
        {
            AmendRecords(models);
        }

        public bool Delete(string model)
        {
            return DeleteXmlElement(model);
        }

        public Login GetLogin(string site)
        {
            if(string.IsNullOrWhiteSpace(site))
            {
                return null;
            }
            return GetLoginSingle(site);
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

        private Login GetLoginSingle(string site)
        {
            if(File.Exists("Logins.xml"))
            {
                XElement doc = XElement.Load("Logins.xml");
                return doc.Elements("Login").Where(l => l.Element("Site").Value.StartsWith(site))
                                            .OrderBy(l => l.Element("Site").Value)
                                            .Select(l => new Login
                                            {
                                                Site = l.Element("Site").Value,
                                                UserName = l.Element("UserName").Value,
                                                Password = l.Element("Password").Value
                                            }).FirstOrDefault();
            }
            return null;
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

        private void AmendRecords(IEnumerable<Login> logins)
        {
            if (!File.Exists("Logins.xml"))
            {
                if (!IsValid(logins.FirstOrDefault())) throw new Exception("login data is not valid");
                AddNewXmlFile(logins.FirstOrDefault());
            }

            var doc = XDocument.Load("Logins.xml");
            UpdateLogins(logins, doc);
            doc.Save("Logins.xml");
        }

        private void UpdateLogins(IEnumerable<Login> logins, XDocument doc)
        {
            foreach (var model in logins)
            {
                UpdateLogin(doc, model);
            }
        }

        private void UpdateLogin(XDocument doc, Login model)
        {
            if (!IsValid(model)) throw new Exception("input data is not valid");

            XElement login =
                  (from lgin in doc.Descendants("Login")
                   where lgin.Element("Site").Value == model.Site
                   select lgin).SingleOrDefault();

            if (login == null)
            {
                login = GetLoginElement(model);
                doc.Root.Add(login);
            }
            else
            {
                login.Element("UserName").Value = model.UserName;
                login.Element("Password").Value = model.Password;
            }
        }

        private void AmendRecord(Login model)
        {
            if (!File.Exists("Logins.xml"))
            {
                if (!IsValid(model)) throw new Exception("input data is not valid");
                AddNewXmlFile(model);
            }
            else
            {
                var doc = XDocument.Load("Logins.xml");
                UpdateLogin(doc, model);
                doc.Save("Logins.xml");
            }
        }

        private bool DeleteXmlElement(string site)
        {
            var doc = XDocument.Load("Logins.xml");

            var login =
                (from lgin in doc.Descendants("Login")
                 where lgin.Element("Site").Value == site
                 select lgin).SingleOrDefault();

            if (login != null)
            {
                login.Remove();
                doc.Save("Logins.xml");
                return true;
            }
            return false;
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
