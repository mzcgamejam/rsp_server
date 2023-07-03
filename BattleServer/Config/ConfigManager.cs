using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Config
{
    public static class ConfigManager
    {
        public static string sqsUrl = "";
        public static string region = "";
        public static string accessKey = "";
        public static string secretKey = "";

        public static void SetConfiguration()
        {
            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = binPath + @"\\config.json";
            StreamReader file = File.OpenText(configPath);
            JsonTextReader reader = new JsonTextReader(file);
            JObject jsonConfig = (JObject)JToken.ReadFrom(reader);
            sqsUrl = (string)jsonConfig["sqs"];
            region = (string)jsonConfig["region"];
            accessKey = (string)jsonConfig["access_key"];
            secretKey = (string)jsonConfig["secret_key"];
        }
    }
}
