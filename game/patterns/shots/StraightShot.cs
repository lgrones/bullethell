using bullethell.game.bullets;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.patterns.shots;

[Tool]
[GlobalClass]
public partial class StraightShot : ShotPattern
{
    public override void Emit(EmitContext ctx, in Transform2D frame)
    {
        var spawn = new BulletSpawn {
            Position = Vector2.Zero,
            Velocity = new Vector2(Speed, 0),
            BehaviorId = ctx.Table.IdFor(Behavior)
        };

        ctx.Sink.SpawnBullet(spawn.Transformed(frame));
    }
}
