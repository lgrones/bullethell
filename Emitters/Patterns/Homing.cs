using System.Collections.Generic;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[GlobalClass]
public sealed partial class Homing : Resources.PatternResource
{
    [Export] public float Speed;
    [Export] public required Resources.BulletStyleResource StyleResource;

    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        if (target is null)
            return;

        sink.Add(Bullet.Create(origin,
            (target.Value - origin).Normalized() * Speed,
            StyleResource.ToStyle()));
    }
}