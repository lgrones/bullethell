using Godot;

namespace bullethell.game.phases;

public partial class BossPhase : Node2D
{
    [Export] public float Duration;
    [Export] public uint Hp;

    public override void _Ready()
        => Disable();

    public void Disable()
        => ProcessMode = ProcessModeEnum.Disabled;

    public void Enable()
        => ProcessMode = ProcessModeEnum.Inherit;
}