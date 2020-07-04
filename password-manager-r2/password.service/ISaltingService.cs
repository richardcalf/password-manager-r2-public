using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.service
{
    public interface ISaltingService
    {
        string ReSalt(string oldSalt, string newSalt, string encryptedValue);
    }
}
