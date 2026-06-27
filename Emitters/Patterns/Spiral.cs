using System.Collections.Generic;
using bullethell.Entities;
using Godot;

namespace bullethell.Emitters.Patterns;

public sealed class Spiral(float startAngle, float turn, float speed, BulletStyle style) : IEmitPattern
{
    private float _angle = startAngle;

    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        _angle += turn;
        sink.Add(Bullet.Create(origin, Vector2.Right.Rotated(_angle) * speed, style));
    }
}