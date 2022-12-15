using System.Data.Entity;

namespace password.model.Database
{
    public class LoginContext : DbContext
    {
        public LoginContext()
        {
            Database.Connection.ConnectionString = "Server=tcp:password.database.windows.net,1433;Initial Catalog=AligatorFarm;Persist Security Info=False;User ID=richard.calf;Password={DATABASE_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //test filter
        }
        public DbSet<LoginModel> Logins { get; set; }
    }
}
