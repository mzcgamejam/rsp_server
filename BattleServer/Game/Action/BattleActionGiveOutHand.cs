﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public class BattleActionGiveOutHand
    {
        public BattleActionResult DoAction()
        {
            return new BattleActionResult() { IsDoing = false };
        }
    }
}
