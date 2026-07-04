using bullethell.game.bullets;
using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.emitters;

public readonly struct EmitContext(IBulletSink sink, BehaviorTable table, StyleTable styles, Vector2 targetPosition, bool hasTarget, float time)
{
    public readonly IBulletSink Sink = sink;
    public readonly BehaviorTable Table = table;
    public readonly StyleTable Styles = styles;
    public readonly Vector2 TargetPosition = targetPosition;
    public readonly bool HasTarget = hasTarget;
    public readonly float Time = time;
}