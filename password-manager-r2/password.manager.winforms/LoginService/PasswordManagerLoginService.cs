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

        public bool Login(Login login)
        {
            login.Site = "admin.admin";
            return loginService.Login(login);
        }

        public bool Register(Login login)
        {
            login.Site = "admin.admin";
            return loginService.Register(login);
        }
    }
}
