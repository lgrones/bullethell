using Godot;

namespace bullethell.game.bullets;

public struct BulletSpawn
{
    public Vector2 Position;
    public Vector2 Velocity;
    public byte BehaviorId;
    public byte StyleId;

    public readonly BulletSpawn Transformed(in Transform2D transform) => this with
    {
        Position = transform * Position,
        Velocity = transform.BasisXform(Velocity),
    };
}