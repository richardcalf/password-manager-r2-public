using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model.Database;

namespace password.model
{
    public class DatabasePersistence : PersistenceValidator, IRepository
    {
        public bool Delete(string site)
        {
            using (var context = new LoginContext())
            {
                var login = context.Logins.FirstOrDefault(l => l.Site.Equals(site));
                if (login != null)
                {
                    context.Logins.Remove(login);
                    return context.SaveChanges() > 0;
                }
                return false;
            }
        }

        public Login GetLogin(string site)
        {
            using (var context = new LoginContext())
            {
                return context.Logins.Where(l => l.Site.StartsWith(site)) 
                                     .OrderBy(l => l.Site)
                                     .Select(l => new Login
                                     {
                                         Site = l.Site,
                                         UserName = l.UserName,
                                         Password = l.Password
                                     }).FirstOrDefault();
            }
        }

        public IEnumerable<Login> GetLogins()
        {
            using (var context = new LoginContext())
            {
                return context.Logins.OrderBy(l => l.Site)
                           .Select(l => new Login
                           {
                               Site = l.Site,
                               UserName = l.UserName,
                               Password = l.Password
                           }).ToList();
            }
        }

        public void Save(Login model)
        {
            using (var context = new LoginContext())
            {
                //we never update a site. only the other details.
                var login = context.Logins.FirstOrDefault(l => l.Site.Equals(model.Site));
                if(login != null)
                {
                    login.UserName = model.UserName;
                    login.Password = model.Password;
                }
                else
                {
                    var newLogin = new LoginModel
                    {
                        Site = model.Site,
                        Password = model.Password,
                        UserName = model.UserName
                    };
                    context.Logins.Add(newLogin);
                }
                context.SaveChanges();
            }
        }

        public void Save(IEnumerable<Login> models)
        {
            foreach(var model in models)
            {
                Save(model);
            }
        }
    }
}
