using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.service;
using password.settings;
using password.resalter;
using password.model;

namespace password.uibroker
{
    public class UIBroker : IUIBroker
    {

        private IResalterAsync resalter;
        public event Action<string> SettingSaved;
        public event Action<string> Resalted;
        public event Action<bool> DataReady;
        public event Action<string> DataUpdate;

        public IRepository Repo { get; }
        public ILoginService LoginService { get; }
        public IEncryptionServiceAsync EncryptionService { get; set; }

        public IEnumerable<Login> Logins { get; set; }

        public string Salt { get; set; }

        public UIBroker(IRepository repo, ILoginService loginService, IResalterAsync resalter)
        {
            Salt = Settings.GetValueFromSettingKey("salt");
            EncryptionService = EncryptionServiceFactory.GetEncryptionServiceAsync(Salt);
            this.resalter = resalter;
            Repo = repo;
            LoginService = loginService;
        }

        public async Task<string> DecryptAsync(string encryptedValue)
        {
            return await EncryptionService.DecryptAsync(encryptedValue);
        }

        public async Task<string> EncryptAsync(string value)
        {
            return await EncryptionService.EncryptAsync(value);
        }

        private void GetAllRecords()
        {
            Logins = Repo.GetLogins();
        }

        public async Task GetAllRecordsAsync()
        {
            DataReady?.Invoke(false);
            await Task.Run(() =>
            {
                GetAllRecords();
            });
            DataReady?.Invoke(true);
        }

        public async Task ResaltAllLogins(string previousSalt, string newSalt)
        {
            Logins = Repo.GetLogins();
            var newSaltedLogins = await resalter.ResaltAsync(previousSalt, newSalt, Logins);
            await Task.Run(() =>
            {
                Repo.Save(newSaltedLogins);
            });
            Logins = newSaltedLogins;
            Settings.SaveAppSetting("salt", newSalt);
            EncryptionService = new EncryptionService(newSalt);
            Resalted?.Invoke(newSalt);
            DataUpdate?.Invoke($"update resalted all sites");
        }

        public void SaveSetting(string key, string value)
        {
            Settings.SaveAppSetting(key, value);
            SettingSaved?.Invoke(key);
        }

        public void RefreshEncryptionSalt()
        {
            Salt = Settings.GetValueFromSettingKey("salt");
            EncryptionService = new EncryptionService(Salt);
        }

        public string GenerateRndPasswrd()
        {
            return Guid.NewGuid().ToString().Substring(0, 13);
        }

        public Login GetLogin(string site)
        {
            return Repo.GetLogin(site);
        }

        public void Save(Login model)
        {
            Repo.Save(model);
            DataUpdate?.Invoke( $"{model.Site}" );
        }

        public bool Delete(string site)
        {
            var result = Repo.Delete(site);
            if (result)
                DataUpdate?.Invoke($"{site} deleted");
            return result;
        }
    }
}
