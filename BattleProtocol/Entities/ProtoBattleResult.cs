using CommonType;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleProtocol.Entities
{
    [MessagePackObject]
    public class ProtoBattleResult : BaseProtocol
    {
        [Key(1)]
        public PlayerType Winner;

        [Key(2)]
        public bool IsBattleEnd;

        [Key(3)]
        public long RoundStartDateTimeTicks;

        [Key(4)]
        public int RoundSecond;
    }
}
