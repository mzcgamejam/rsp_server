using CommonType;
using MessagePack;

namespace BattleProtocol.Entities
{
    [MessagePackObject]
    public class ProtoBattleEnterResult : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public PlayerType PlayerType;

        [Key(3)]
        public long GameStartDateTimeTicks;

        [Key(4)]
        public int GameSecond;
    }
}
