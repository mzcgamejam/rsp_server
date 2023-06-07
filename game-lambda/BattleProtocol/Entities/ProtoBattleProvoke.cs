using CommonType;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleProtocol.Entities
{
    [MessagePackObject]
    public class ProtoBattleProvoke : BaseProtocol
    {
        [Key(1)]
        public string GameSessionId;

        [Key(2)]
        public PlayerType PlayerType;

        [Key(3)]
        public int ProvokeType;
    }
}
