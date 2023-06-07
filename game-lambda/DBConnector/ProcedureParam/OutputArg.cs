using MySqlConnector;

namespace GameDB
{
    public class OutputArg
    {
        public string Key { get; set; }
        public MySqlDbType ValueType { get; set; }

        public OutputArg(string key, MySqlDbType dbType)
        {
            Key = key;
            ValueType = dbType;
        }
    }
}
