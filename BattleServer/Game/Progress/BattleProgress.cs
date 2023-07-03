using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleTimer.BattleTimer;

namespace BattleServer.Game
{
    public class BattleProgress : IDisposable
    {
        private readonly Timer _progressTimer;
        public void TimerStart() { _progressTimer.Start(); }
        public void TimerStop() { _progressTimer.Stop(); }
        public void Dispose() { _progressTimer.Dispose(); }

        private readonly object _lockObject = new object();
        private readonly List<IBattleProgress> _progresses = new List<IBattleProgress>()
        {
            new BattleProgressEnd(),
            new BattleProgressRoundEnd()
        };

        public enum ProgressType
        {
            End,
            RoundEnd
        }

        public BattleProgress( )
        {
            _progressTimer = new Timer(10, true, BattleProgressTimerElapsed)
            {
                Name = "progress"
            };
        }

        private void BattleProgressTimerElapsed()
        {
            if (!System.Threading.Monitor.TryEnter(_lockObject))
                return;
            try
            {
                bool didProcess = _progresses.Any(progress =>
                {
                    if (progress.IsProgress())
                    {
                        progress.Update();
                        return true;
                    }
                    return false;
                });

                if (!didProcess)
                {
                    var nextProgress = _progresses.FirstOrDefault(progress => progress.HasWork());
                    nextProgress?.Update();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BattleProgress", ex);
                Console.WriteLine("\t[BattleProgress Exception :" + ex.GetType().ToString() + ex.Message + ex.StackTrace);
            }
            finally
            {
                System.Threading.Monitor.Exit(_lockObject);
            }
        }


        public void EnqueueEnd(BattleActionEnd battleAction)
        {
            _progresses[(int)ProgressType.End].EnqueueAction(battleAction);
        }

        public void EnqueueRoundEnd(BattleActionRoundEnd battleAction)
        {
            _progresses[(int)ProgressType.RoundEnd].EnqueueAction(battleAction);
        }
    }
}
