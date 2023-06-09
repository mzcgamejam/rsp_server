using BattleProtocol;
using BattleProtocol.Entities;
using BattleServer.Server;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    class TestController : BaseController
    {
        public override async Task DoPipeline(BattleSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoTest;

            session.Send(req.Msg, MessagePackSerializer.Serialize(new ProtoTest
            {
                Msg = MessageType.Test,
                Num = 20
            }));
        }
    }
}
