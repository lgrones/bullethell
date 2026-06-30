namespace bullethell.game.core.timers;

public class IntervalTimer(float time, int maxTicks = 8)
{
    private float _time = time;

    public int MaxTicks { get; } = maxTicks;
    public float Elapsed { get; private set; }

    // A shot is due whenever the countdown has run out.
    public bool Due => _time <= 0f;

    public void SetInterval(float interval)
        => _time = interval;

    public void AddTime(float additionalTime)
        => _time += additionalTime;

    public void Advance(float delta)
    {
        Elapsed += delta;
        _time -= delta;
    }
}
