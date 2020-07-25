using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using password.service;
using System.Threading.Tasks;

namespace password.encryption.tests
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void test_encryption_and_decryption_service()
        {
            var input = "password1";
            IEncryptionService c = new EncryptionService();
            var encrypted = c.Encrypt(input);
            var decrypted = c.Decrypt(encrypted);
            Assert.IsTrue(decrypted == input);
        }

        [TestMethod]
        public async Task test_encryption_and_decryption_service_async()
        {
            var input = "PassWrd";
            IEncryptionServiceAsync service = new EncryptionService();
            var encrypted = await service.EncryptAsync(input);
            var decrypted = await service.DecryptAsync(encrypted);
            Assert.IsTrue(decrypted == input);
        }
    }
}
