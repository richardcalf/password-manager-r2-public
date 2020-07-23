using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model
{
    public class DatabasePersistence : IRepository
    {
        public bool Delete(string site)
        {
            throw new NotImplementedException();
        }

        public Login GetLogin(string site)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Login> GetLogins()
        {
            throw new NotImplementedException();
        }

        public bool IsValid(Login model)
        {
            throw new NotImplementedException();
        }

        public void Save(Login model)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<Login> models)
        {
            throw new NotImplementedException();
        }
    }
}
