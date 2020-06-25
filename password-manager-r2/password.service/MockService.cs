using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace password.service
{
    public class MockService : IService, IServiceAsync
    {
        public string Decrypt(string value)
        {
            return "decrypted";
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
            return "encrypted";
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