using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.service
{
    public interface IServiceAsync
    {
        Task<string> DecryptAsync(string value);
        Task<string> EncryptAsync(string value);
    }
}
