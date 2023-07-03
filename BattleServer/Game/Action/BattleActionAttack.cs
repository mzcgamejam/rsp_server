using CommonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public class BattleActionAttack : BattleAction
    {
        private Action<PlayerType, AttackType> _battleAttack;
        private readonly PlayerType _playerType;
        private readonly AttackType _attackType;

        public BattleActionAttack(Action<PlayerType, AttackType> battleAttack, PlayerType playerType, AttackType attackType) : base(null)
        {
            _battleAttack = battleAttack;
            _playerType = playerType;
            _attackType = attackType;
        }

        public override BattleActionResult DoAction()
        {
            _battleAttack(_playerType, _attackType);
            _battleAttack = null;

            return new BattleActionResult() { IsDoing = true };
        }
    }
}
