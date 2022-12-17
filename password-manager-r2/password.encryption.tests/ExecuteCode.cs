using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.encryption.tests.Code.Runner.Classes;
using password.model;
using password.service;

namespace password.encryption.tests
{
    [TestClass]
    public class ExecuteCode
    {
        [TestMethod]
        public void copy_xml_data_do_db()
        {
            var xmlLogins = CodeRunner.xmlrepo.GetLogins().ToList();

            CodeRunner.dbrepo.Save(xmlLogins);

            Assert.IsTrue(true);
        }
        [TestMethod]
        public void copy_db_data_to_xml()
        {
            var dbLogins = CodeRunner.dbrepo.GetLogins().ToList();

            CodeRunner.xmlrepo.Save(dbLogins);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task random_db_passwords()
        {
            var dbLogins = CodeRunner.dbrepo.GetLogins().ToList();

            foreach(var login in dbLogins)
            {
                var rndPass = CodeRunner.GetRandomString();
                //now default salt encrypt it
                IEncryptionServiceAsync EncryptionService = new EncryptionService();
                rndPass = await EncryptionService.EncryptAsync(rndPass);
                if (login.Site != "admin.admin")
                {
                    login.Password = rndPass;
                }
            }

            CodeRunner.dbrepo.Save(dbLogins);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task random_xml_passwords()
        {
            var xmlLogins = CodeRunner.xmlrepo.GetLogins().ToList();

            foreach (var login in xmlLogins)
            {
                var rndPass = CodeRunner.GetRandomString();
                //now default salt encrypt it
                IEncryptionServiceAsync EncryptionService = new EncryptionService();
                rndPass = await EncryptionService.EncryptAsync(rndPass);
                if (login.Site != "admin.admin")
                {
                    login.Password = rndPass;
                }
            }

            CodeRunner.xmlrepo.Save(xmlLogins);

            Assert.IsTrue(true);
        }
    }
}
