using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http.Headers;

namespace password.model
{
    public class XmlPersistence : IRepository
    {
        public Login GetLogin(string site)
        {
            List<Login> logins = new List<Login>();

            XmlDocument doc = new XmlDocument();
            if (File.Exists("Logins.xml"))
            {
                doc.Load("Logins.xml");
                IterateOverLogins(logins, doc);
            }

            return logins.FirstOrDefault(l => l.Site.Equals(site));
        }

        private static void IterateOverLogins(List<Login> logins, XmlDocument doc)
        {
            var Site = string.Empty;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode lgIn in node)
                {
                    if (lgIn.Name.Equals("Site"))
                    {
                        Site = lgIn.InnerText;
                        var lgin = logins.FirstOrDefault(l => l.Site.Equals(Site));
                        if (lgin == null)
                        {
                            logins.Add(new Login { Site = Site });
                        }
                    }

                    if (lgIn.Name.Equals("UserName"))
                    {
                        var lgin = logins.FirstOrDefault(l => l.Site.Equals(Site));
                        if (lgin != null)
                        {
                            lgin.UserName = lgIn.InnerText;
                        }
                    }

                    if (lgIn.Name.Equals("Password"))
                    {
                        var lgin = logins.FirstOrDefault(l => l.Site.Equals(Site));
                        if (lgin != null)
                        {
                            lgin.Password = lgIn.InnerText;
                        }
                    }
                }
            }
        }

        public void Save(Login model)
        {
            PersistXmlFile(model);
        }

        private void PersistXmlFile(Login model)
        {
            if(!IsValid(model)) throw new Exception("Fill out all fields.");
            
            if (!File.Exists("Logins.xml"))
            {
                AddNewXmlFile(model);
            }
            else
            {
                XmlDocument doc;
                if(!AmendRecord(model, out doc))
                {
                    AddNewRecord(model, doc);
                }
                doc.Save("Logins.xml");
            }
        }

        private static void AddNewRecord(Login model, XmlDocument doc)
        {
            //add the Login/site to the file
            var root = doc.DocumentElement;
            XmlNode loginNode = doc.CreateNode(XmlNodeType.Element, "Login", "");

            XmlNode siteNode = doc.CreateNode(XmlNodeType.Element, "Site", "");
            siteNode.InnerText = model.Site;
            loginNode.AppendChild(siteNode);

            XmlNode userNameNode = doc.CreateNode(XmlNodeType.Element, "UserName", "");
            userNameNode.InnerText = model.UserName;
            loginNode.AppendChild(userNameNode);

            XmlNode passwordNode = doc.CreateNode(XmlNodeType.Element, "Password", "");
            passwordNode.InnerText = model.Password;
            loginNode.AppendChild(passwordNode);

            root.AppendChild(loginNode);
        }

        private static bool AmendRecord(Login model, out XmlDocument doc)
        {
            doc = new XmlDocument();
            doc.Load("Logins.xml");
            var Site = string.Empty;
            bool fileHasSite = false;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode lgIn in node)
                {
                    if (lgIn.Name.Equals("Site"))
                    {
                        if (lgIn.InnerText.Equals(model.Site))
                        {
                            fileHasSite = true;
                            Site = model.Site;
                        }
                        else
                        {
                            Site = string.Empty;
                        }
                    }
                    if (lgIn.Name.Equals("UserName"))
                    {
                        if (Site.Equals(model.Site))
                        {
                            lgIn.InnerText = model.UserName;
                        }
                    }
                    if (lgIn.Name.Equals("Password"))
                    {
                        if (Site.Equals(model.Site))
                        {
                            lgIn.InnerText = model.Password;
                        }
                    }
                }
            }
            return fileHasSite;
        }

        private static void AddNewXmlFile(Login model)
        {
            var document = new XDocument();
            var logins = new XElement("Logins");
            
            var login = new XElement("Login", new XElement("Site", model.Site),
                new XElement("UserName", model.UserName), 
                new XElement("Password", model.Password));//functional construction
            logins.Add(login);

            document.Add(logins);
            document.Save("Logins.xml");
        }

        public IEnumerable<Login> GetLogins()
        {
            XmlDocument doc = new XmlDocument();
            List<Login> logins = new List<Login>();
            if (File.Exists("Logins.xml"))
            {
                doc.Load("Logins.xml");
            }
            else
            {
                return new List<Login>();
            }

            var Site = string.Empty;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode lgIn in node)
                {
                    if (lgIn.Name.Equals("Site"))
                    {
                        Site = lgIn.InnerText;
                        logins.Add(new Login { Site = lgIn.InnerText });
                    }

                    if (lgIn.Name.Equals("UserName"))
                    {
                        var lgin = logins.FirstOrDefault(l => l.Site.Equals(Site));
                        if (lgin != null)
                        {
                            lgin.UserName = lgIn.InnerText;
                        }
                    }

                    if (lgIn.Name.Equals("Password"))
                    {
                        var lgin = logins.FirstOrDefault(l => l.Site.Equals(Site));
                        if (lgin != null)
                        {
                            lgin.Password = lgIn.InnerText;
                        }
                    }
                }
            }
            return logins.OrderBy(l => l.Site);
        }

        public bool IsValid(Login model)
        {
            return !(string.IsNullOrWhiteSpace(model.Site) ||
                string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.Password));
                
        }
    }
}
