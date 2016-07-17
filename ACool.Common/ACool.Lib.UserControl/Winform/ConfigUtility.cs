using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class ConfigUtility
    {
        public static void Add(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;

            app.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static Dictionary<string,string> Query()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;

            return app.Settings.Cast<KeyValueConfigurationElement>().ToDictionary(x => x.Key, x => x.Value);
        }

        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void Update(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;

            app.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static void Delete(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;
            
            app.Settings.Remove(key);
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static Dictionary<string, string> QueryConnectionStrings(bool ExceptLocalSqlServer = true)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConnectionStringsSection ConnectionStrings = config.ConnectionStrings;

            return ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().Where(x =>
            {
                if (ExceptLocalSqlServer)
                {
                    return x.Name != "LocalSqlServer";
                }
                else
                {
                    return true;
                }
            }
            ).ToDictionary(x => x.Name, x => x.ConnectionString);

        }
    }
}
