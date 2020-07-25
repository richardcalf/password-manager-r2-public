using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model;
using password.service;
using password.settings;

namespace password.encryption.tests.Code.Runner.Classes
{
    public static class CodeRunner
    {
        public static IRepository repo = new XmlPersistence();
        private static Random random = new Random();
        static string globalSalt = Settings.GetValueFromSettingKey("salt");
        static IEncryptionService encryptionService = EncryptionServiceFactory.GetEncryptionService(globalSalt);

        public static Login GetRandomLogin()
        {
            Login login = new Login();
            login.Site = RandomString(10);
            login.UserName = RandomString(10);
            string plainPassword = RandomString(20);
            string encryptedPassword = encryptionService.Encrypt(plainPassword);
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
