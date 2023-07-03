using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public abstract class BattleAction
    {
        public Player Attacker;

        public BattleAction(Player attacker)
        {
            Attacker = attacker;
        }

        public abstract BattleActionResult DoAction();
    }
}
