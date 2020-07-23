using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using password.model;
using password.model.Database;

namespace password.encryption.tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
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
            Assert.IsTrue(login != null);
        }

        [TestMethod]
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
            Assert.IsTrue(logins.Count > 1);
        }

        [TestMethod]
        public void test_login_fetch()
        {
            IRepository repo = new XmlPersistence();
            Login login = repo.GetLogin("amainhobbies.com");

            if (login == null)
            {
                Assert.IsNull(login);
            }
            else
            {
                Assert.IsTrue(!string.IsNullOrEmpty(login.UserName));
                Assert.IsTrue(login.Password == "a8AzC8EGlsLvyFev5GOI/g==");
            }
        }

        [TestMethod]
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

            Assert.IsTrue(saves > 0);
        }

        [TestMethod]
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

            Assert.IsTrue(saves > 0);

        }
    }
}
