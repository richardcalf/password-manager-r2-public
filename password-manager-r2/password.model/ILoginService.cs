using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model
{
    public interface ILoginService
    {
        bool Register(Login login);

        bool Login(Login login);
    }
}
