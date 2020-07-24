using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model
{
    public abstract class PersistenceValidator
    {
        public bool IsValid(Login model)
        {
            return !(string.IsNullOrWhiteSpace(model.Site) ||
                string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.Password));
        }
    }
}
