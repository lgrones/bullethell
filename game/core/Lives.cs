using Godot;

namespace bullethell.game.core;

/// Run-wide life count. Emits Changed so the HUD row can react, and GameOver
/// once the last life is spent so Main can reset without polling.
public partial class Lives : RefCounted
{
    [Signal]
    public delegate void ChangedEventHandler(int current, int max);

    [Signal]
    public delegate void GameOverEventHandler();

    public int Current { get; private set; }
    public int Max { get; private set; }

    public void Reset(int count)
    {
        Max = Current = count;
        EmitSignal(SignalName.Changed, Current, Max);
    }

    public void Lose()
    {
        if (Current <= 0)
            return;

        Current--;
        EmitSignal(SignalName.Changed, Current, Max);

        if (Current <= 0)
            EmitSignal(SignalName.GameOver);
    }
}
