using System;

namespace CommonConfig
{
    public class EnvironmentReader
    {
        public Infos infos = new Infos();
        private static EnvironmentReader _instance = null;

        public static EnvironmentReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EnvironmentReader();
                    _instance.infos.DB.IP = Environment.GetEnvironmentVariable("GAME_DB_IP");
                    _instance.infos.DB.Port = uint.Parse(Environment.GetEnvironmentVariable("GAME_DB_PORT") ?? "0");
                    _instance.infos.DB.User = Environment.GetEnvironmentVariable("GAME_DB_USER");
                    _instance.infos.DB.Password = Environment.GetEnvironmentVariable("GAME_DB_PASSWORD");
                    _instance.infos.DB.Charset = Environment.GetEnvironmentVariable("GAME_DB_CHARSET");
                    _instance.infos.DB.Database = Environment.GetEnvironmentVariable("GAME_DB_DATABASE");

                    _instance.infos.GameLift.FleetId = Environment.GetEnvironmentVariable("GAMELIFT_FLEET_ID");
                    _instance.infos.GameLift.ServiceURL = Environment.GetEnvironmentVariable("GAMELIFT_SERVICE_URL");
                }
                return _instance;
            }
        }
    }
}
