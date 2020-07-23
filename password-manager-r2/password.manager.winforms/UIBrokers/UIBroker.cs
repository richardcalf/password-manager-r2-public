﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.service;
using password.settings;
using password.resalter;
using password.model;

namespace password.manager.winforms
{
    public class UIBroker
    {
        private IServiceAsync service;
        private readonly IResalterAsync resalter;
        public event Action<string> SettingSaved;
        public event Action<string> Resalted;
        public event Action<bool> DataReady;

        public IRepository Repo { get; }

        public IEnumerable<model.Login> Logins { get; private set; }

        public string Salt { get; private set; }

        public string PushPath
        {
            get
            {
                return Settings.GetValueFromSettingKey("push");
            }
        }

        public UIBroker()
        {
            Salt = Settings.GetValueFromSettingKey("salt");
            service = EncryptionServiceFactory.GetEncryptionServiceAsync(Salt);
            resalter = new Resalter();
            Repo = new DatabasePersistence();
            _ = GetAllRecordsAsync();
        }

        public async Task<string> DecryptAsync(string encryptedValue)
        {
            return await service.DecryptAsync(encryptedValue);
        }

        public async Task<string> EncryptAsync(string value)
        {
            return await service.EncryptAsync(value);
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
            service = new EncryptionService(newSalt);
            Resalted?.Invoke(newSalt);
        }

        public void SaveSetting(string key, string value)
        {
            Settings.SaveAppSetting(key, value);
            SettingSaved?.Invoke(key);
        }

        public void RefreshEncryptionSalt()
        {
            Salt = Settings.GetValueFromSettingKey("salt");
            service = new EncryptionService(Salt);
        }

        
    }
}
