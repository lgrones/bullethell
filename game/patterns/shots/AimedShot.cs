using bullethell.game.bullets;
using bullethell.game.bullets.behaviors;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.patterns.shots;

[Tool]
[GlobalClass]
public partial class AimedShot : Pattern
{
    [Export] public BulletBehavior? Behavior;
    [Export] public float Speed = 200f;

    public override void Emit(EmitContext ctx, in Transform2D frame)
    {
        if (!ctx.HasTarget) return;
        
        var direction = (ctx.TargetPosition - frame.Origin).Normalized();

        ctx.Sink.SpawnBullet(new BulletSpawn
        {
            Position = Vector2.Zero,
            Velocity = direction * Speed,
            BehaviorId = ctx.Table.IdFor(Behavior)
        }.Transformed(frame));
    }
}