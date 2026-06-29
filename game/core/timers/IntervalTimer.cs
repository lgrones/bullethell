namespace bullethell.game.core.timers;

public class IntervalTimer(float time, int maxTicks = 8)
{
    private float _time = time;
    public float Elapsed { get; private set; }

    public void SetInterval(float interval)
        => _time = interval;

    public void AddTime(float additionalTime)
        => _time += additionalTime;

    public int Update(float delta)
    {
        Elapsed += delta;
        _time -= delta;

        // while, not if — catch up, no dropped shots at low fps      
        var ticks = 0;
        while (_time <= 0f && ticks < maxTicks)
        {
            ticks++;
        }

        return ticks;
    }
}