using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.encryption.tests.Code.Runner.Classes;
using password.model;

namespace password.encryption.tests
{
    [TestClass]
    public class ExecuteCode
    {
        [TestMethod]
        public void test_some_code()
        {
            List<Login> logins = new List<Login>();

            for (int i = 0; i < 1000; i++)
            {
                var login = CodeRunner.GetRandomLogin();
                logins.Add(login);
            }
            CodeRunner.repo.Save(logins);
            Assert.IsTrue(true);
        }
    }
}
