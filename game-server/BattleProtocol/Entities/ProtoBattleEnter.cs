using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleProtocol.Entities
{
    [MessagePackObject]
    public class ProtoBattleEnter : BaseProtocol
    {
        [Key(1)]
        public string GameSessionId;

        [Key(2)]
        public string PlayerSessionId;

        [Key(3)]
        public int UserId;
    }
}
