using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model;

namespace password.login.service
{
    public interface ILoginService
    {
        bool Register(Login login);

        bool Login(Login login);
    }
}
