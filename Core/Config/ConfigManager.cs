using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace TaoTray.Core.Config
{
    internal class ConfigManager
    {
        private string ConfigPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "config.json");
        public ConfigManager() { }
        public ConfigModel GetConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    using (StreamReader sr = new StreamReader(ConfigPath))
                    {
                        var cfg_txt = sr.ReadToEnd();
                        var cfg = JsonSerializer.Deserialize<ConfigModel>(cfg_txt);
                        if (cfg != null) App.AppConfig = cfg;
                    }
                    return App.AppConfig;
                }
                else
                {
                    throw new FileNotFoundException("Config file not found.");
                }
            }catch(Exception e)
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_CONFIG_NOT_VALID, e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new FileNotFoundException("Config file not valid.");
            }

        }

        public void SaveConfig(ConfigModel? config = null)
        {
            if (config != null) App.AppConfig = config;
            var json = JsonSerializer.Serialize<ConfigModel>(App.AppConfig, new JsonSerializerOptions() { WriteIndented = true });

            using (StreamWriter sw = new StreamWriter(ConfigPath))
            {
                sw.Write(json);
            }
        }
    }
}
