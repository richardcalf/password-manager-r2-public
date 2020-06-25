using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using password.service;

namespace password.encryption.tests
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void test_encryption_and_decryption_service()
        {
            var input = "password1";
            IService c = new EncryptionService();
            var encrypted = c.Encrypt(input);
            var decrypted = c.Decrypt(encrypted);
            Assert.IsTrue(decrypted == input);
        }
    }
}
