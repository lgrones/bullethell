using System;
using bullethell.game.bullets.behaviors;
using bullethell.game.core;

namespace bullethell.game.bullets.systems;

/// Tests live bullets against a single target (player or boss). On overlap the
/// bullet dies and OnHit fires once — the field turns that into a Godot signal,
/// so damage plumbing stays at the actor layer while the bullet loop stays POCO.
public sealed class CollisionSystem : IBulletSystem
{
    private readonly StyleTable _styles;

    /// Set each frame by the field. Null = nothing to hit (e.g. editor preview).
    public ICollidable? Target;

    /// Invoked once per bullet that connects. Kept alloc-free: no args, no damage
    /// payload — one hit == one unit, the actor decides what that costs.
    public Action? OnHit;

    public CollisionSystem(StyleTable styles) => _styles = styles;

    public void Run(BulletPool pool, in BulletFrame frame)
    {
        if (Target is null) return;

        var span = pool.Active;
        var styles = _styles.Styles;

        for (var i = 0; i < span.Length; i++)
        {
            ref var bullet = ref span[i];
            var hitRadius = styles[bullet.StyleId].HitRadius;

            // frame.Target is the target's world position; Target.Position is the
            // node's local one, which only matches when its parent sits at origin.
            // Rooms are offset, so the boss's local pos is wrong — use the world one.
            if (!ICollidable.Overlaps(frame.Target, Target.HitRadius, bullet.Position, hitRadius))
                continue;

            bullet.Alive = false;
            OnHit?.Invoke();
        }
    }
}
