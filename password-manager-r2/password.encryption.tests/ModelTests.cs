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
                Site = "testSite.com",
                UserName = "richard.calf@gmail.com"
                ,
                Password = "Yg/bW1YkWfOrQ8RrfDVRCw=="
            };

            repo.Save(login);
            Assert.IsTrue(login != null);
        }
    }
}
