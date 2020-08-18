using System.Data.Entity;

namespace password.model.Database
{
    public class LoginContext : DbContext
    {
        public LoginContext() : base("CometGalaxy")
        {

        }
        public DbSet<LoginModel> Logins { get; set; }
    }
}
