using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.manager.winforms
{
    public class UILoginBroker
    {

        IPasswordManagerLoginService loginService;

        public UILoginBroker()
        {
            loginService = new PasswordManagerLoginService();
        }
    }
}
