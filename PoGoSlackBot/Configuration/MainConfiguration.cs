using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Configuration
{
    public class MainConfiguration
    {
        public string MapURLFormat { get; set; }

        public string ImageURLFormat { get; set; }

        public List<InstanceConfiguration> Instances { get; set; }

        public MainConfiguration()
        {
            Instances = new List<InstanceConfiguration>();
        }

        public static MainConfiguration Load()
        {
            string configPath = Path.Combine(GetDataDirectory(), "settings.json");
            if (!File.Exists(configPath))
                throw new FileNotFoundException("Configuration file not found", configPath);

            try
            {
                var configJson = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<MainConfiguration>(configJson);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid configuration file.");
            }
        }

        private static string GetDataDirectory()
        {
            string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(text))
            {
                text = AppDomain.CurrentDomain.BaseDirectory;
            }
            return text;
        }
    }
}
