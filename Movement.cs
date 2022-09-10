using System;
using System.Windows.Threading;

namespace DbClock;
class Movement {

    public event EventHandler Tick;

    public DateTime Time { get; private set; }

    public DateTime Interpolated { get; private set; }

    public TimeSpan Offset { get; private set; }

    public TimeSpan TimerResolution { get; set; } = TimeSpan.FromMilliseconds(16.7);

    public TimeSpan TimerUpdateTime { get; set; } = TimeSpan.FromSeconds(0.1);

    public Func<DateTime> GetTime { get; set; } = () => DateTime.Now;

    public Movement(Dispatcher dispatcher = null, DispatcherPriority priority = DispatcherPriority.Render) {
        dispatcher ??= Dispatcher.CurrentDispatcher;
        _ = new DispatcherTimer(TimerResolution, priority, OnTimer, dispatcher);
    }

    void OnTimer(object sender, EventArgs e) {
        var timer = sender as DispatcherTimer;
        SetTime(GetTime());
        if (timer.Interval == TimerResolution) {
            if (Offset < TimerResolution) timer.Interval = TimerUpdateTime;
        }
        else if (timer.Interval > TimerResolution) Tick?.Invoke(this, EventArgs.Empty);
    }

    void SetTime(DateTime time) {
        var secondStartTick = (time.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond;
        Offset = TimeSpan.FromTicks(time.Ticks - secondStartTick);
        Interpolated = time.Add(-Offset);
        Time = time;
    }

}