namespace bullethell.game.core.timers;

public class Timer(float duration)
{
    private float _elapsed;
    private float _duration = duration;
    
    public bool IsElapsed { get; private set; }
    public float Remaining => _duration - _elapsed > 0f ? _duration - _elapsed : 0f;
    public float Duration => _duration;

    public void Update(float delta)
    {
        _elapsed += delta;
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