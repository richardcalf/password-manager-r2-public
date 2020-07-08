using password.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.login.service;

namespace password.manager.winforms
{
    public class PasswordManagerLoginService : IPasswordManagerLoginService
    {
        ILoginService loginService;

        public PasswordManagerLoginService()
        {
            loginService = new XmlLoginService();
        }

        public bool Login(model.Login login)
        {
            login.Site = "admin.admin";
            return loginService.Login(login);
        }

        public bool Register(model.Login login)
        {
            login.Site = "admin.admin";
            return loginService.Register(login);
        }
    }
}
