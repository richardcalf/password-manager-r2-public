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

    public class EncryptionService : IService, IServiceAsync
    {
        public string Decrypt(string value)
        {
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
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
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
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
