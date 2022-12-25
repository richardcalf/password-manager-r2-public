using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using password.settings;

namespace password.settings
{
    public static class Settings
    {
        public static string GetValueFromSettingKey(string value)
        {
            var doc = XElement.Load(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
            return
                (from setting in doc.Elements("appSettings").Elements("add")
                 select setting).Where(s => s.Attribute("key").Value == value)
                                  .Select(p => p.Attribute("value").Value).FirstOrDefault();
        }

        public static void SaveAppSetting(string key, string value)
        {
            try
            {
                var configFile = GetConfigurationFile();

                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private static Configuration GetConfigurationFile()
        {
            Configuration configFile;
            configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return configFile;
        }

        public static void RemoveAppSettings(List<string> keys)
        {
            try
            {
                var configFile = GetConfigurationFile();
                var settings = configFile.AppSettings.Settings;
                foreach (var key in keys)
                {
                    if (settings[key] != null)
                    {
                        settings.Remove(key);
                    }
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
