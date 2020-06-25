using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using password.encryption;

namespace password.encryption.tests
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void test_encryption_and_decryption()
        {
            var input = "password1";
            Crypto c = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
            var encrypted = c.Encrypt(input);
            var decrypted = c.Decrypt(encrypted);
            Assert.IsTrue(decrypted == input);
        }
    }
}
