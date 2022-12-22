using password.model;
using password.model.Database;

namespace password.xunit.tests
{
    public class ModelTests
    {
        [Fact]
        public void test_login_creation()
        {
            XmlPersistence repo = new XmlPersistence();
            Login login = new Login
            {
                Site = "amainhobbies.com",
                UserName = "insano"
                ,
                Password = "a8AzC8EGlsLvyFev5GOI/g=="
            };

            repo.Save(login);
            Assert.True(login != null);
        }

        [Fact]
        public void test_many_logins_creation()
        {
            IRepository repo = new XmlPersistence();
            var logins = new List<Login> { new Login { Site = "site1.com",
                                                       UserName = "richard",
                                                       Password = "qfFUHUgnXHiYX0yEkbLAQQ==" },
                                           new Login { Site = "site2.com",
                                                       UserName = "jack", Password = "4GdfQZA92cx8AvhVLcFSDg==" },
                                           new Login { Site = "site3.com", UserName = "jane", Password = "HWuxhCtwAI1brFOa2+c4Zw==" }};

            repo.Save(logins);
            Assert.True(logins.Count > 1);
        }

        [Fact]
        public void test_login_fetch()
        {
            IRepository repo = new XmlPersistence();
            Login login = repo.GetLogin("amainhobbies.com");

            if (login == null)
            {
                Assert.Null(login);
            }
            else
            {
                Assert.True(!string.IsNullOrEmpty(login.UserName));
                Assert.True(login.Password == "a8AzC8EGlsLvyFev5GOI/g==");
            }
        }

        [Fact]
        public void test_new_database_record()
        {

            int saves = 0;
            var login = new LoginModel
            {
                UserName = "test.username",
                Site = "test.com",
                Password = "Q/NTlrXHkso8AcFGR2AkKw=="
            };
            using (var context = new LoginContext())
            {
                context.Logins.Add(login);
                saves = context.SaveChanges();
            }

            Assert.True(saves > 0);
        }

        [Fact]
        public void test_new_database_record_from_plain_jo()
        {
            int saves = 0;
            var loginPojo = new Login
            {
                Site = "abc.com",
                UserName = "richard.calf@gmail.com",
                Password = "SEjT7fbb6b8kRWPtQ2i/fQ=="
            };

            var login = new LoginModel
            {
                Site = loginPojo.Site,
                UserName = loginPojo.UserName,
                Password = loginPojo.Password
            };
            using (var context = new LoginContext())
            {
                context.Logins.Add(login);
                saves = context.SaveChanges();
            }

            Assert.True(saves > 0);
        }

        [Fact]
        public void test_save_many_database_records()
        {
            int saves = 0;
            var logins = new List<Login> { new Login { Site = "madmax.com",
                                                       UserName = "richard.calf",
                                                       Password = "aOJPaq8Ih5wGWlS7axT1aw==" },
                                           new Login { Site = "def.com",
                                                       UserName = "jane.calf", Password = "aOJPaq8Ih5wGWlS7axT1aw==" },
                                           new Login { Site = "adt.com", UserName = "jane.calf@gmail.com", Password = "aOJPaq8Ih5wGWlS7axT1aw==" }};
            using (var context = new LoginContext())
            {
                context.Logins.AddRange(logins.Select(l => new LoginModel
                {
                    Site = l.Site,
                    UserName = l.UserName,
                    Password = l.Password
                }));
                saves = context.SaveChanges();
                Assert.True(saves > 0);
            }
        }

        [Fact]
        public void test_get_a_login()
        {
            var site = "madmax.com";
            Login login = null;
            using (var context = new LoginContext())
            {
                var lgin = context.Logins.FirstOrDefault(l => l.Site.Equals(site));
                if (lgin != null)
                {
                    login = new Login { Site = lgin.Site, UserName = lgin.Site, Password = lgin.Password };
                }
            }
            Assert.NotNull(login);
        }
    }
}
