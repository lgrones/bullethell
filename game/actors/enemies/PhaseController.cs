using bullethell.game.core.health;
using bullethell.game.core.timers;
using Godot;

namespace bullethell.game.actors.enemies;

/// The stable hub of a boss fight. Owns Health + Countdown and forwards their
/// signals so listeners (ring, HUD) bind once. Advancing a phase is driven by
/// either HP depletion or the timer elapsing — neither listener needs the other.
public partial class PhaseController : RefCounted
{
    /// Raised so the Boss node can build the phase scene for this index.
    [Signal]
    public delegate void PhaseEnteredEventHandler(int index);

    /// Raised after the last phase ends.
    [Signal]
    public delegate void DefeatedEventHandler();

    public readonly Health Health = new();
    public readonly Countdown Timer = new();

    private readonly int _count;
    private int _index = -1;

    public PhaseController(int count)
    {
        _count = count;
        Health.Depleted += Next;
        Timer.Elapsed += Next;
    }

    public void Start()
    {
        _index = -1;
        Next();
    }

    /// Called by the Boss once the phase scene is built, feeding its HP/duration.
    public void Configure(int hp, float duration)
    {
        Health.Reset(hp);
        Timer.Start(duration);
    }

    public void Update(float delta) => Timer.Update(delta);

    public void Damage() => Health.Damage();

    private void Next()
    {
        _index++;

        if (_index >= _count)
        {
            EmitSignal(SignalName.Defeated);
            return;
        }

        EmitSignal(SignalName.PhaseEntered, _index);
    }
}
