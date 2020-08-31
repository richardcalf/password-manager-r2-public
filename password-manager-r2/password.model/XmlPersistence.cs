using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using password.settings;

namespace password.model
{
    public class XmlPersistence : PersistenceValidator, IRepository, ILoginService
    {

        private string loginFilePath;

        public XmlPersistence()
        {
            loginFilePath = Settings.GetValueFromSettingKey("loginFilePath");
            if(loginFilePath == null)
            {
                loginFilePath = "Logins.xml";
                Settings.SaveAppSetting("loginFilePath", loginFilePath);
            }
        }

        #region IRepository
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
        #endregion

        #region ILoginService
        public bool Login(Login login)
        {
            return LoginExists(login);
        }

        public bool Register(Login login)
        {
            if (File.Exists(loginFilePath))
            {
                File.Delete(loginFilePath);
            }
            Save(login);
            return true;
        }
        #endregion

        #region private methods

        private Login GetLoginSingle(string site)
        {
            if(File.Exists(loginFilePath))
            {
                XElement doc = XElement.Load(loginFilePath);
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
            if (File.Exists(loginFilePath))
            {
                XElement doc = XElement.Load(loginFilePath);

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
            if (!File.Exists(loginFilePath))
            {
                if (!IsValid(logins.FirstOrDefault())) throw new Exception("input data is not valid");
                AddNewXmlFile(logins.FirstOrDefault());
            }

            var doc = XDocument.Load(loginFilePath);
            UpdateLogins(logins, doc);
            doc.Save(loginFilePath);
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
            if (!File.Exists(loginFilePath))
            {
                if (!IsValid(model)) throw new Exception("input data is not valid");
                AddNewXmlFile(model);
            }
            else
            {
                var doc = XDocument.Load(loginFilePath);
                UpdateLogin(doc, model);
                doc.Save(loginFilePath);
            }
        }

        private bool DeleteXmlElement(string site)
        {
            var doc = XDocument.Load(loginFilePath);

            var login =
                (from lgin in doc.Descendants("Login")
                 where lgin.Element("Site").Value == site
                 select lgin).SingleOrDefault();

            if (login != null)
            {
                login.Remove();
                doc.Save(loginFilePath);
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
            doc.Save(loginFilePath);
        }

        private XElement GetLoginElement(Login model)
        {
            return new XElement("Login",
                                 new XElement("Site", model.Site),
                                 new XElement("UserName", model.UserName),
                                 new XElement("Password", model.Password));
        }

        private bool LoginExists(Login model)
        {
            var doc = XDocument.Load(loginFilePath);
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
