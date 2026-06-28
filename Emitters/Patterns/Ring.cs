using System.Collections.Generic;
using bullethell.Emitters.Resources;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[Tool]
[GlobalClass]
public sealed partial class Ring : PatternResource
{
    [Export] public int Arms;
    [Export] public float Speed;
    [Export] public required BulletStyleResource StyleResource;

    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        for (var i = 0; i < Arms; i++)
            sink.Add(Bullet.Create(origin,
                Vector2.Right.Rotated(Mathf.Tau * i / Arms) * Speed,
                StyleResource.ToStyle()));
    }
}