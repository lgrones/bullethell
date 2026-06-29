namespace bullethell.game.bullets;

public enum MovementKind : byte
{
    Straight,
    Homing
}

public enum TransitionKind : byte
{
    Never,
    AfterTime,
    NearTarget
}

public enum DeathAction : byte
{
    Despawn,
    Explode
}

public struct StateDefinition
{
    public MovementKind Move;
    public float TurnRate;
    public TransitionKind Exit;
    public float ExitThreshold;
    public DeathAction OnEnd;
    public int ExplodeCount;
    public float ExplodeSpeed;
}