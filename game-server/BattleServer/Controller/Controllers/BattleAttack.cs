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
    class BattleAttack : BaseController
    {
        public override async Task DoPipeline(BattleSession session, BaseProtocol requestInfo)
        {
            Console.WriteLine("BattleAttack Console");
            Debug.WriteLine("BattleAttack Debug");


            using (StreamWriter outputFile = new StreamWriter(@"C:\Game\myserver222.txt", true))
            {
                outputFile.WriteLine("BattleAttack");
            }


            var req = requestInfo as ProtoBattleAttack;
            RoomManager.BattleAttack(req.GameSessionId, req.PlayerType, req.AttackType);
        }
    }
}
