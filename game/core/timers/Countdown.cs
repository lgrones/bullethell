using Godot;

namespace bullethell.game.core.timers;

/// Signal-emitting wrapper over the POCO Timer. Ticked fires every Update so a
/// progress bar can drain; Elapsed fires once when the duration runs out.
public partial class Countdown : RefCounted
{
    [Signal]
    public delegate void TickedEventHandler(float remaining, float duration);

    [Signal]
    public delegate void ElapsedEventHandler();

    private readonly Timer _timer = new(0f);

    public void Start(float duration) => _timer.Reset(duration);

    public void Update(float delta)
    {
        var was = _timer.IsElapsed;
        _timer.Update(delta);
        EmitSignal(SignalName.Ticked, _timer.Remaining, _timer.Duration);

        if (!was && _timer.IsElapsed)
            EmitSignal(SignalName.Elapsed);
    }
}
