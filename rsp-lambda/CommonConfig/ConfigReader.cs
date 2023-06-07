using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ConfigReader
{
    private static ConfigReader _instance = null;

    public static ConfigReader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConfigReader();
            }
            return _instance;
        }
    }

    private string _runmode;

    public void Init(string runMode = "Local")
    {
        _runmode = runMode;
    }

    public T GetInfos<T>()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        while(true)
        {
            var configPath = $@"{basePath}/Config/{_runmode}/GameServerConfig.json";
            if (File.Exists(configPath))
            {
                var jsonString = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            basePath = System.IO.Directory.GetParent(basePath).FullName;
        }
    }
}
