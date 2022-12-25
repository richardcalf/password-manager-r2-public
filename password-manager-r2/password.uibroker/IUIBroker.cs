using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using password.model;
using password.service;

namespace password.uibroker
{
    public interface IUIBroker
    {

        ILoginService LoginService { get; }

        IEncryptionServiceAsync EncryptionService { get; set; }

        IRepository Repo { get; }

        event Action<string> SettingSaved;

        event Action<string> Resalted;

        event Action<bool> DataReady;

        string Salt { get; set; }

        IEnumerable<Login> Logins { get; set; }

        #region methods

        Task<string> DecryptAsync(string encryptedValue);

        Task<string> EncryptAsync(string value);

        Task GetAllRecordsAsync();

        Task ResaltAllLogins(string previousSalt, string newSalt);

        void SaveSetting(string key, string value);

        void RefreshEncryptionSalt();

        string GenerateRndPasswrd();
        #endregion
    }
}
