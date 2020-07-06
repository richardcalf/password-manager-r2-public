using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using password.model;
using password.service;
using password.resalter;
using password.settings;

namespace password.code.runner
{
    class Program
    {
        static IRepository repo = new XmlPersistence();
        static IService service;
        static string globalSalt = Settings.GetValueFromSettingKey("salt");

        private static Random random = new Random();

        static void Main(string[] args)
        {
            service = new EncryptionService(globalSalt);

            List<Login> logins = new List<Login>();

            for (int i = 0; i < 10; i++)
            {
                var login = GetRandomLogin();
                logins.Add(login);
            }
            repo.Save(logins);
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static Login GetRandomLogin()
        {
            Login login = new Login();

            login.Site = RandomString(10);
            login.UserName = RandomString(10);
            string plainPassword = RandomString(20);
            string encryptedPassword = service.Encrypt(plainPassword);
            login.Password = encryptedPassword;

            return login;
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
