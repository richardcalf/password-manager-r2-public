using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using password.model;

namespace password.code.runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //var repo = new XmlPersistence();
            //repo.CreateXmlFile(new Login { Site = "google.com", UserName = "richard.calf", Password = "lfoVwiwPZ9pRkPuoaZBINw==" });

            //XmlDocument xdoc = new XmlDocument();
            //xdoc.Load("Logins.xml");
            //xdoc.Save(Console.Out);

            List<Login> logins = new List<Login>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Logins.xml");

            var Site = string.Empty;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode login in node)
                {
                    if (login.Name.Equals("Site"))
                    {
                        Site = login.InnerText;
                        var lgin = logins.Where(l => l.Site.Equals(Site)).FirstOrDefault();
                        if (lgin == null)
                        {
                            logins.Add(new Login { Site = Site });
                        }
                        Console.WriteLine($"Site = {login.InnerText}");
                    }
                    if (login.Name.Equals("UserName"))
                    {
                        var lgin = logins.Where(l => l.Site.Equals(Site)).FirstOrDefault();
                        if(lgin != null)
                        {
                            lgin.UserName = login.InnerText;
                        }

                        Console.WriteLine($"UserName = {login.InnerText}");
                    }
                    if (login.Name.Equals("Password"))
                    {
                        var lgin = logins.Where(l => l.Site.Equals(Site)).FirstOrDefault();
                        if(lgin != null)
                        {
                            lgin.Password = login.InnerText;
                        }
                        Console.WriteLine($"Password = {login.InnerText}");
                    }
                }
            }
            Console.ReadLine();
        }

        private static void UseXmlTextReader()
        {
            XmlTextReader reader = new XmlTextReader("Logins.xml");
            List<Login> logins = new List<Login>();
            while (reader.Read())
            {
                string site = string.Empty;
                string user = string.Empty;
                string password = string.Empty;
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Site")
                {
                    site = reader.ReadElementContentAsString();

                    Console.WriteLine($"\tSite = {site}");
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "UserName")
                {
                    user = reader.ReadElementContentAsString();
                    Console.WriteLine($"\tUserName = {user}");
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Password")
                {
                    password = reader.ReadElementContentAsString();
                    Console.WriteLine($"\tPassword = {password}");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    if (!string.IsNullOrEmpty(site) && !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(password))
                        logins.Add(new Login { Site = site, UserName = user, Password = password });
                }
            }
        }
    }
}
