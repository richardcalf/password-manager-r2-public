using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace password.model
{
    public class XmlPersistence : IRepository
    {
        public Login GetLogin(string site)
        {
            List<Login> logins = new List<Login>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Logins.xml");

            var Site = string.Empty;
            foreach(XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach(XmlNode lgIn in node)
                {
                    if(lgIn.Name.Equals("Site"))
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
            var login = logins.FirstOrDefault(l => l.Site.Equals(site));
            if(login != null)
            {
                return login;
            }
            return new Login();
        }

        public int Save(Login model)
        {
            CreateXmlFile(model);
            return 1;
        }

        public void CreateXmlFile(Login model)
        {
            var document = new XDocument();
            var logins = new XElement("Logins");
            //--//
            var site = new XElement("Site", model.Site);
            var username = new XElement("UserName", model.UserName);
            var password = new XElement("Password", model.Password);
            var login = new XElement("Login", site, username, password);//functional construction
            logins.Add(login);

            document.Add(logins);
            document.Save("Logins.xml");
        }

    }
}
