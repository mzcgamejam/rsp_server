namespace GameDB
{
    public class InputArg
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public InputArg(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
