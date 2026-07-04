using Godot;

namespace bullethell.game.core.health;

/// Pooled HP state for a fightable actor. Emits on every change so the ring
/// healthbar and the phase-advance rule can each react without knowing about
/// one another.
public partial class Health : RefCounted
{
    [Signal]
    public delegate void ChangedEventHandler(int current, int max);

    /// Raised once when Current reaches 0.
    [Signal]
    public delegate void DepletedEventHandler();

    public int Current { get; private set; }
    public int Max { get; private set; }
    public float Fraction => Max == 0 ? 0f : (float)Current / Max;

    public void Reset(int max)
    {
        Max = max;
        Current = max;
        EmitSignal(SignalName.Changed, Current, Max);
    }

    public void Damage(int amount = 1)
    {
        if (Current <= 0)
            return;

        Current = Mathf.Max(0, Current - amount);
        EmitSignal(SignalName.Changed, Current, Max);

        if (Current == 0)
            EmitSignal(SignalName.Depleted);
    }
}
