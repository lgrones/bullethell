using System.Collections.Generic;
using bullethell.Entities;
using Godot;

namespace bullethell.Emitters.Patterns;

public sealed class Homing(float speed, BulletStyle style) : IEmitPattern
{
    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
        => sink.Add(Bullet.Create(origin, (ctx.PlayerPosition - origin).Normalized() * speed, style));
}