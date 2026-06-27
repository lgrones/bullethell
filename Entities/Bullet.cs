using Godot;

namespace bullethell.Entities;

public readonly record struct BulletStyle(float Radius, float HitRadius, Color Color);

public struct Bullet
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; init; }
    public BulletStyle Style { get; private init; }

    public static Bullet Create(Vector2 position, Vector2 velocity, BulletStyle style) => new()
    {
        Position = position,
        Velocity = velocity,
        Style = style
    };

    public bool IsInBounds(Vector2 viewPort)
    {
        var margin = Style.Radius * 3;

        return Position.X >= -margin
               && Position.X <= viewPort.X + margin
               && Position.Y >= -margin
               && Position.Y <= viewPort.Y + margin;
    }
}