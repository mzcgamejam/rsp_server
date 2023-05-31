using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Game.Progress
{
    public class BattleProgressGiveOutHand : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionGiveOutHand> _action 
            = new ConcurrentQueue<BattleActionGiveOutHand>();

        public void EnqueueAction<T>(T action)
        {
            _action.Enqueue(action as BattleActionGiveOutHand);
        }

        public bool HasWork()
        {
            return _action.IsEmpty == false;
        }

        public bool IsProgress()
        {
            return HasWork();
        }

        public void Update()
        {
            if (_action.TryDequeue(out var action))
            {
                action.DoAction();
            }
        }
    }
}
