using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BattleTimer
{
    public class BattleTimer
    {
        public class Timer : IDisposable
        {
            public string Name;
            protected System.Timers.Timer _timer;
            private TimerStatus _timerStatus = TimerStatus.Stop;
            private DateTime _pauseDateTime;
            private double _intervalAfterOneTemporary;
            private bool _autoReset;
            private bool _disposed = false;

            protected Action _action;
            protected DateTime _recentEventDateTime;
            protected double _nowInterval;
            public bool IsTimerStatusStop => _timerStatus == TimerStatus.Stop;
            public bool IsTimerStatusPause => _timerStatus == TimerStatus.Pause;

            public double NowInterval => _nowInterval;
            public double IntervalAfterOneTemporary { set { _intervalAfterOneTemporary = value; } }

            private object _lock = new object();


            public Timer(double interval, bool autoReset, Action action)
            {
                _timer = new System.Timers.Timer();
                _nowInterval = interval;
                _autoReset = autoReset;
                _action = action;
                _timer.AutoReset = _autoReset;
                _timer.Elapsed += TimerElapsed;
                _timer.Disposed += TimerDisposed;
            }

            public void SetAttribute(double interval, bool autoReset)
            {
                _nowInterval = interval;
                _timer.AutoReset = autoReset;
            }

            public void SetAttribute(double interval, bool autoReset, Action action)
            {
                _nowInterval = interval;
                _timer.AutoReset = autoReset;
                _action = action;
            }

            public virtual void Start()
            {
                if (_disposed)
                    return;

                _timerStatus = TimerStatus.Proceeding;
                _recentEventDateTime = DateTime.UtcNow;
                _timer.AutoReset = _autoReset;

                lock (_lock)
                {
                    AkaTimerStart();
                }
            }

            public void Stop()
            {
                _timerStatus = TimerStatus.Stop;
                _timer.Stop();
            }

            public void ReInterval(double interval)
            {
                _recentEventDateTime = DateTime.UtcNow;
                _nowInterval = interval;

                lock (_lock)
                {
                    AkaTimerStart();
                }

            }

            public virtual void Pause()
            {
                lock (_lock)
                {
                    if (_timerStatus == TimerStatus.Proceeding || _timerStatus == TimerStatus.Restart)
                    {
                        _timer.Stop();

                        //if (_timerStatus == TimerStatus.Proceeding)
                        _timerStatus = TimerStatus.Pause;

                        _pauseDateTime = DateTime.UtcNow;
                    }
                }
            }

            public virtual void Restart(int bulletTime, double forcingSetInverval = 0)
            {
                if ((_timerStatus != TimerStatus.Pause && _timerStatus != TimerStatus.Restart) || (_nowInterval == 0 && forcingSetInverval == 0) || _disposed)
                    return;

                TimerElapsedActionChange();
                TimerIntervalChange(forcingSetInverval);
                _timer.AutoReset = false;
                RecentEventDateTimeChange(bulletTime);

                _timerStatus = TimerStatus.Restart;
                _timer.Start();
            }

            private void TimerElapsedActionChange()
            {
                //if (_timerStatus == TimerStatus.Restart)
                _timer.Elapsed -= TimerRestartElapsed;

                _timer.Elapsed -= TimerElapsed;
                _timer.Elapsed += TimerRestartElapsed;
            }

            private void TimerIntervalChange(double forcingSetInverval)
            {
                if (forcingSetInverval == 0)
                    _timer.Interval = GetRestartFirstInterval();
                else
                    _timer.Interval = forcingSetInverval;

            }

            private void RecentEventDateTimeChange(int bulletTime)
            {
                if (bulletTime == 0)
                    _recentEventDateTime = DateTime.UtcNow;
                else
                    _recentEventDateTime = _recentEventDateTime.AddMilliseconds(bulletTime);
            }

            private double GetRestartFirstInterval()
            {
                var betweenInterval = (_pauseDateTime - _recentEventDateTime).TotalMilliseconds;
                return (_nowInterval - betweenInterval) > 0 ? _nowInterval - betweenInterval : 1;
            }

            public void Close()
            {
                _timer.Close();
            }

            public void Dispose()
            {
                lock (_lock)
                {
                    _disposed = true;
                    _timer.Stop();
                    _timer.Dispose();
                }
            }

            System.Threading.ManualResetEvent _disposeEvent = null;
            public void DisposeWait()
            {
                _disposeEvent = new System.Threading.ManualResetEvent(false);
                Dispose();
                _disposeEvent.WaitOne();
                _disposeEvent.Dispose();

            }

            private void TimerDisposed(object sender, EventArgs e)
            {
                _disposeEvent?.Set();
            }

            private void TimerElapsed(object sender, ElapsedEventArgs e)
            {
                //if( Name != "progress")
                //    Logger.Instance().Info("TimerElapsed");
                _recentEventDateTime = DateTime.UtcNow;
                _action();
                ElapsedEnd();
            }

            private void TimerRestartElapsed(object sender, ElapsedEventArgs e)
            {
                lock (_lock)
                {
                    if (_timerStatus == TimerStatus.Pause)
                        return;

                    _timerStatus = TimerStatus.Proceeding;
                    _recentEventDateTime = DateTime.UtcNow;
                    _timer.Elapsed -= TimerRestartElapsed;
                    _timer.Elapsed += TimerElapsed;
                    _timer.AutoReset = _autoReset;
                    if (_intervalAfterOneTemporary != 0)
                    {
                        _nowInterval = _intervalAfterOneTemporary;
                        _intervalAfterOneTemporary = 0;
                    }

                    if (_autoReset)
                    {
                        AkaTimerStart();
                    }

                    _action();
                    ElapsedEnd();
                }
            }

            private void AkaTimerStart()
            {
                if (_nowInterval > 0)
                {
                    try
                    {
                        if (_disposed == false)
                        {
                            _timer.Interval = _nowInterval; 
                            _timer.Start();                 
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("TimerStart : ", ex);
                        Console.WriteLine($"[TimerSTart] {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }

            private void ElapsedEnd()
            {
                if (_autoReset == false)
                    _timerStatus = TimerStatus.Stop;
            }

        }
    }
}
