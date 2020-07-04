using password.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.service;


namespace password.resalter
{
    public class Resalter : IResalter, IResalterAsync
    {
        public IEnumerable<Login> Resalt(string previousSalt, string newSalt, IEnumerable<Login> logins)
        {
            
            IService originalSaltService = GetPrevious(previousSalt);
            IService resaltService = new EncryptionService(newSalt);
            List<Login> newSaltLoginList = new List<Login>();

            foreach (var login in logins)
            {
                var password = originalSaltService.Decrypt(login.Password);
                login.Password = resaltService.Encrypt(password);
                newSaltLoginList.Add(login);
            }
            return newSaltLoginList;
        }

        public Task<IEnumerable<Login>> ResaltAsync(string previousSalt, string newSalt,
            IEnumerable<Login> logins)
        {
            return Task.Run(() =>
            {
                return Resalt(previousSalt, newSalt, logins);
            });
        }

        private EncryptionService GetPrevious(string previousSalt)
        {
            if (string.IsNullOrWhiteSpace(previousSalt))
            {
                return new EncryptionService();
            }
            else
            {
                return new EncryptionService(previousSalt);
            }
        }
    }
}
