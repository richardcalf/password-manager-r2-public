using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace password.service
{
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
}