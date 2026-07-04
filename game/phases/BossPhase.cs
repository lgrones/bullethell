using Godot;

namespace bullethell.game.phases;

/// Root of a packed phase scene: holds its duration and HP, and parents the
/// emitters that fire while it's alive. The boss instantiates it to start the
/// phase and frees it to end — no enable/disable needed.
public partial class BossPhase : Node2D
{
    [Export] public float Duration;
    [Export] public uint Hp;
}
