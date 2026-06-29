using Godot;

namespace bullethell.game.core;

public interface ICollidable
{
    Vector2 Position { get; }
    float HitRadius { get; }

    bool IsHitBy(ICollidable other) 
        => Overlaps(this, other);
    
    bool IsHitBy(Vector2 position, float hitRadius) 
        => Overlaps(Position, HitRadius, position, hitRadius);

    static bool Overlaps(Vector2 aPos, float aR, Vector2 bPos, float bR)
    {
        var r = aR + bR;
        return aPos.DistanceSquaredTo(bPos) < r * r;
    }

    static bool Overlaps(ICollidable a, ICollidable b)
        => Overlaps(a.Position, a.HitRadius, b.Position, b.HitRadius);
}