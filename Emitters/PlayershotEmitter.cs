using System.Collections.Generic;
using bullethell.Entities.Bullets;
using bullethell.Timers;
using Godot;

namespace bullethell.Emitters;

public sealed class PlayerShotEmitter(float interval, float speed, BulletStyle style)
{
    private readonly IntervalTimer _timer = new(interval);

    public void Update(Vector2 origin, List<PlayerShot> sink, in FrameContext ctx)
    {
        var ticks = _timer.Update(in ctx);

        while (ticks-- > 0)
            sink.Add(PlayerShot.Create(origin, Vector2.Up * speed, style));
    }
}