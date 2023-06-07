using MessagePack;

namespace BattleProtocol
{
    [MessagePackObject]
    public class BaseProtocol
    {
        [Key(0)]
        public MessageType Msg { get; set; }
    }
}
