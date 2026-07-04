using bullethell.game.bullets;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.patterns.shots;

[Tool]
[GlobalClass]
public partial class AimedShot : ShotPattern
{
    public override void Emit(EmitContext ctx, in Transform2D frame)
    {
        if (!ctx.HasTarget) return;

        // Direction is already in global space, so spawn directly at the
        // emitter's global origin without re-applying its rotation/scale basis
        // (Transformed would double-rotate the velocity — wrong for rotated/
        // spinning emitters).
        var direction = (ctx.TargetPosition - frame.Origin).Normalized();

        ctx.Sink.SpawnBullet(new BulletSpawn
        {
            Position = frame.Origin,
            Velocity = direction * Speed,
            BehaviorId = ctx.Table.IdFor(Behavior),
            StyleId = ctx.Styles.IdFor(Style)
        });
    }
}
