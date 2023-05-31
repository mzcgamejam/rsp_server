using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleProtocol.Entities
{
    [MessagePackObject]
    public class ProtoTest : BaseProtocol
    {
        [Key(1)]
        public int Num;
    }
}
