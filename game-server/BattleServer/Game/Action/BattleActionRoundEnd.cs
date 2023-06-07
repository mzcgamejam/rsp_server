using CommonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public class BattleActionRoundEnd : BattleAction
    {
        private Action<PlayerType, bool> _roundEnd;
        private readonly PlayerType _winner;
        private readonly bool _isRetreat;

        public BattleActionRoundEnd(Action<PlayerType, bool> roundEnd, PlayerType winner, bool isRetreat) : base(null)
        {
            _roundEnd = roundEnd;
            _winner = winner;
            _isRetreat = isRetreat;
        }

        public override BattleActionResult DoAction()
        {
            _roundEnd(_winner, _isRetreat);
            _roundEnd = null;

            return new BattleActionResult() { IsDoing = true };
        }
    }
}
