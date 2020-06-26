using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using password.model;

namespace password.encryption.tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void test_login_creation()
        {
            IRepository repo = new XmlPersistence();
            Login login = new Login
            {
                Site = "hireOurTools.com",
                UserName = "richard.calf@gmail.com"
                ,
                Password = "as9vxCuBr18T2NmX0sMSHQ=="
            };

            repo.Save(login);
            Assert.IsTrue(login != null);
        }

        [TestMethod]
        public void test_login_fetch()
        {
            IRepository repo = new XmlPersistence();
            Login login = repo.GetLogin("timetravel.com");

            Assert.IsTrue(!string.IsNullOrEmpty(login.UserName));
            Assert.IsTrue(login.Password == "YJYABFM5GXp5yee17Kdg1YA1l8onY7iv");
        }
    }
}
