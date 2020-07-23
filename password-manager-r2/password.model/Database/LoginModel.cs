using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.model.Database
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string Site { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
