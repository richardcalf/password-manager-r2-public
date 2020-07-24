using password.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model.Database;

namespace password.login.service
{
    public class DatabaseLoginService : ILoginService
    {
        public bool Login(Login login)
        {
            return LoginExists(login);
        }

        public bool Register(Login login)
        {
            using (var context = new LoginContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [LoginModels]");
                var lgin = new LoginModel
                {
                    Site = login.Site,
                    UserName = login.UserName,
                    Password = login.Password
                };
                context.Logins.Add(lgin);
                return context.SaveChanges() > 0;
            }
        }

        private bool LoginExists(Login model)
        {
            using (var context = new LoginContext())
            {
                return context.Logins
                    .FirstOrDefault(l => l.Site.Equals(model.Site) &&
                                               l.UserName.Equals(model.UserName) &&
                                               l.Password.Equals(model.Password)) != null;
            };
        }
    }
}
