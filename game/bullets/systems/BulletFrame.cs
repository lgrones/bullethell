using Godot;

namespace bullethell.game.bullets.systems;

/// Per-frame transient state handed to every system by `in` ref.
/// Persistent data (behavior/style tables, mesh) lives on the systems themselves.
public readonly struct BulletFrame(float dt, float time, Vector2 target, bool hasTarget, Rect2 bounds)
{
    public readonly float Dt = dt;
    public readonly float Time = time;
    public readonly Vector2 Target = target;
    public readonly bool HasTarget = hasTarget;
    public readonly Rect2 Bounds = bounds;
}
