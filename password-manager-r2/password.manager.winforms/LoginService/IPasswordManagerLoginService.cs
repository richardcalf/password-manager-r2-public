using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model;

namespace password.manager.winforms
{
    public interface IPasswordManagerLoginService
    {
        bool Register(model.Login login);

        bool Login(model.Login login);
    }
}
