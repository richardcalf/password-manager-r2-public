using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model;

namespace password.resalter
{
    public interface IResalter
    {
        IEnumerable<Login> Resalt(string previous, string @new, IEnumerable<Login> logins);
    }
}
