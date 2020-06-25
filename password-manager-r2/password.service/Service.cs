using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using password.encryption;

namespace password.service
{
    public interface IService
    {
        string Decrypt(string value);
        string Encrypt(string value);
    }
    public class MockService : IService
    {
        public string Decrypt(string value)
        {
            return "decrypted";
        }

        public string Encrypt(string value)
        {
            return "encrypted";
        }
    }

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
