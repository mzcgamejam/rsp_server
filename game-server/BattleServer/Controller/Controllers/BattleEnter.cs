using BattleProtocol;
using BattleProtocol.Entities;
using BattleServer.Game;
using BattleServer.Server;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    class BattleEnter : BaseController
    {
        public override async Task DoPipeline(BattleSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBattleEnter;
            session.UserId = req.UserId;
            session.GameSessionId = req.GameSessionId;
            session.PlayerSessionId = req.PlayerSessionId;
            RoomManager.BattleEnter(session);
        }
    }
}
