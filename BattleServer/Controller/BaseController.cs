using BattleProtocol;
using BattleServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    public abstract class BaseController
    {
        public abstract Task DoPipeline(BattleSession session, BaseProtocol requestInfo);
    }
}
