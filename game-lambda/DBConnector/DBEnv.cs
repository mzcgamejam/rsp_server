using CommonConfig;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace GameDB
{
    public class DBEnv
    {
        public static SortedDictionary<string, SortedDictionary<int, MySqlConnectionStringBuilder>> SettingBuilderMap
            = new SortedDictionary<string, SortedDictionary<int, MySqlConnectionStringBuilder>>();


        public static void SetUp()
        {
            SetUpEnvironment("Accounts", 0);
        }

        public static void SetUp(string dbVariety, int shardNum)
        {
            if (SettingBuilderMap.ContainsKey(dbVariety) == false)
                SettingBuilderMap.Add(dbVariety, new SortedDictionary<int, MySqlConnectionStringBuilder>());

            if (SettingBuilderMap[dbVariety].ContainsKey(shardNum) == false)
                SettingBuilderMap[dbVariety].Add(shardNum, new MySqlConnectionStringBuilder());

            
            var infos = ConfigReader.Instance.GetInfos<Infos>();
            Console.WriteLine("IP:" + infos.DB.IP);
            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Port = (uint)infos.DB.Port;
            stringBuilder.Server = infos.DB.IP;
            stringBuilder.Database = infos.DB.Database;
            stringBuilder.UserID = infos.DB.User;
            stringBuilder.Password = infos.DB.Password;
            stringBuilder.Pooling = true;
            stringBuilder.MinimumPoolSize = 10;
            stringBuilder.MaximumPoolSize = 100;
            stringBuilder.SslMode = MySqlSslMode.None;
            stringBuilder.CharacterSet = infos.DB.Charset;

            SettingBuilderMap[dbVariety][shardNum] = stringBuilder;
        }

        public static void SetUpEnvironment(string dbVariety, int shardNum)
        {
            if (SettingBuilderMap.ContainsKey(dbVariety) == false)
                SettingBuilderMap.Add(dbVariety, new SortedDictionary<int, MySqlConnectionStringBuilder>());

            if (SettingBuilderMap[dbVariety].ContainsKey(shardNum) == false)
                SettingBuilderMap[dbVariety].Add(shardNum, new MySqlConnectionStringBuilder());

            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Port = EnvironmentReader.Instance.infos.DB.Port;
            stringBuilder.Server = EnvironmentReader.Instance.infos.DB.IP;
            stringBuilder.Database = EnvironmentReader.Instance.infos.DB.Database;
            stringBuilder.UserID = EnvironmentReader.Instance.infos.DB.User;
            stringBuilder.Password = EnvironmentReader.Instance.infos.DB.Password;
            stringBuilder.Pooling = true;
            stringBuilder.MinimumPoolSize = 10;
            stringBuilder.MaximumPoolSize = 100;
            stringBuilder.SslMode = MySqlSslMode.None;
            stringBuilder.CharacterSet = EnvironmentReader.Instance.infos.DB.Charset;

            SettingBuilderMap[dbVariety][shardNum] = stringBuilder;
        }
    }
}
