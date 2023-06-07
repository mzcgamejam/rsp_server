using BattleProtocol;
using BattleProtocol.Entities;
using BattleServer.Game;
using BattleServer.Server;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    class BattleProvoke : BaseController
    {
        public override async Task DoPipeline(BattleSession session, BaseProtocol requestInfo)
        {
            Console.WriteLine("BattleProvoke Console");
            Debug.WriteLine("BattleProvoke Debug");


            using (StreamWriter outputFile = new StreamWriter(@"C:\Game\myserver222.txt", true))
            {
                outputFile.WriteLine("BattleProvoke");
            }


            var req = requestInfo as ProtoBattleProvoke;
            RoomManager.BattleProvoke(req.GameSessionId, req.PlayerType, req.ProvokeType);
        }
    }
}
