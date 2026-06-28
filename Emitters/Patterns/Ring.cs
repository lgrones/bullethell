using System.Collections.Generic;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

public sealed class Ring(int arms, float speed, BulletStyle style) : IEmitPattern
{
    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        for (var i = 0; i < arms; i++)
            sink.Add(Bullet.Create(origin, Vector2.Right.Rotated(Mathf.Tau * i / arms) * speed, style));
    }
}