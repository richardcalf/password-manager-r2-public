using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.service
{
    public static class EncryptionServiceFactory
    {
        public static IEncryptionServiceAsync GetEncryptionServiceAsync(string salt)
        {
            if (string.IsNullOrWhiteSpace(salt))
            {
                return new EncryptionService();
            }
            else
            {
                return new EncryptionService(salt);
            }
        }

        public static IEncryptionService GetEncryptionService(string salt)
        {
            if (string.IsNullOrWhiteSpace(salt))
            {
                return new EncryptionService();
            }
            else
            {
                return new EncryptionService(salt);
            }
        }

    }
}
