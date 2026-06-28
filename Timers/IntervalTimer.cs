using System;

namespace bullethell.Timers;

public class IntervalTimer(float interval, int maxTicks = 8)
{
    private float _elapsed;
    private float _interval = interval;

    public void SetInterval(float interval)
        => _interval = interval;

    public int Update(float delta)
    {
        _elapsed += delta;

        // while, not if — catch up, no dropped shots at low fps      
        var ticks = 0;
        while (_elapsed >= _interval)
        {
            _elapsed -= _interval;
            ticks++;
        }

        return Math.Min(ticks, maxTicks);
    }
}