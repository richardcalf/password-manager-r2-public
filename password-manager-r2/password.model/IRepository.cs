using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model
{
    public interface IRepository
    {
        void Save(Login model);
        bool Delete(string site);
        Login GetLogin(string site);
        IEnumerable<Login> GetLogins();
        bool IsValid(Login model);
    }
}
