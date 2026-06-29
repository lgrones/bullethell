using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletState : Resource
{
    [Export] public MovementKind Move;
    [Export] public float TurnRate = 3f;
    [Export] public TransitionKind Exit;
    [Export] public float ExitThreshold;
    [Export] public DeathAction OnEnd;
    [Export] public int ExplodeCount = 12;
    [Export] public float ExplodeSpeed = 150f;
}