using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.service
{
    public static class EncryptionServiceFactory
    {
        public static IServiceAsync GetEncryptionServiceAsync(string salt)
        {
            if (salt == null)
            {
                return new EncryptionService();
            }
            else
            {
                return new EncryptionService(salt);
            }
        }

        public static IService GetEncryptionService(string salt)
        {
            if (salt == null)
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
