using Godot;

namespace bullethell.game.bullets;

public struct BulletSpawn
{
    public Vector2 Position;
    public Vector2 Velocity;
    public byte BehaviorId;

    public readonly BulletSpawn Transformed(in Transform2D f) => this with
    {
        Position = f * Position,
        Velocity = f.BasisXform(Velocity),
    };
}