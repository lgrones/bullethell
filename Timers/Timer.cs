namespace bullethell.Timers;

public class Timer(float duration)
{
    private float _elapsed;
    private float _duration = duration;
    public bool IsElapsed { get; private set; }

    public void Update(in FrameContext ctx)
    {
        _elapsed += ctx.Delta;
        IsElapsed = _elapsed >= _duration;
    }

    public void Reset(float duration)
    {
        _elapsed = 0f;
        IsElapsed = false;
        _duration = duration;
    }

    public void Reset()
        => Reset(_duration);
}