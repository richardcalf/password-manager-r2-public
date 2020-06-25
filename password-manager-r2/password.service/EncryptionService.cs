using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using password.encryption;

namespace password.service
{

    public class EncryptionService : IService
    {
        public string Decrypt(string value)
        {
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
            return crypt.Decrypt(value);
        }

        public string Encrypt(string value)
        {
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
            return crypt.Encrypt(value);
        }
    }
}
