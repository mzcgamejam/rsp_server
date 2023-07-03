using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game
{
    public class BattleProgressEnd : IBattleProgress
    {
        private BattleActionEnd _battleEnd;
        private bool _isDoing = false;
        public void Update()
        {
            if (_battleEnd == null)
                return;

            _isDoing = true;
            _battleEnd?.DoAction();
            _battleEnd = null;
        }

        public bool IsProgress()
        {
            return _isDoing;
        }

        public bool HasWork()
        {
            return _battleEnd != null;
        }

        public void EnqueueAction<T>(T action)
        {
            _battleEnd = action as BattleActionEnd;
        }
    }
}
