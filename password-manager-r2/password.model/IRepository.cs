using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model
{
    public interface IRepository
    {
        int Save(Login model);
        Login GetLogin(string site);

        void CreateXmlFile(Login model);
    }
}
