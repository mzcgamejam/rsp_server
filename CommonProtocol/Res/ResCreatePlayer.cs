namespace CommonProtocol
{
    public class ResCreatePlayer : CBaseProtocol
    {
        public ResponseType ResponseType;

        public string IpAddress;

        public int Port;

        public string PlayerSessionId;
    }
}
