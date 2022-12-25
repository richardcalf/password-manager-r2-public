using password.service;

namespace password.xunit.tests
{
    public class EncryptionTest
    {
        [Fact]
        public void test_encryption_and_decryption_service()
        {
            var input = "password1";
            IEncryptionService c = new EncryptionService();
            var encrypted = c.Encrypt(input);
            var decrypted = c.Decrypt(encrypted);
            Assert.Equal(input, decrypted);
        }

        [Fact]
        public async Task test_encryption_and_decryption_service_async()
        {
            var input = "PassWrd";
            IEncryptionServiceAsync service = new EncryptionService();
            var encrypted = await service.EncryptAsync(input);
            var decrypted = await service.DecryptAsync(encrypted);
            Assert.Equal(decrypted, input);
        }
    }
}