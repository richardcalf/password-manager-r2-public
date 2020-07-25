using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using password.encryption;

namespace password.service
{

    public class EncryptionService : IEncryptionService, IEncryptionServiceAsync
    {
        private Crypto crypt;
        public EncryptionService(string salt)
        {
            crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES, salt);//salt from consumer
        }

        public EncryptionService()
        {
            crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);//default salt
        }

        public string Decrypt(string value)
        {
            return crypt.Decrypt(value);
        }

        public Task<string> DecryptAsync(string value)
        {
            return Task.Run(() =>
            {
                return Decrypt(value);
            });
        }

        public string Encrypt(string value)
        {
            return crypt.Encrypt(value);
        }

        public Task<string> EncryptAsync(string value)
        {
            return Task.Run(() =>
            {
                return Encrypt(value);
            });
        }
    }
}
