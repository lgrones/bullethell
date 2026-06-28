using System.Collections.Generic;
using bullethell.Emitters.Resources;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[Tool]
[GlobalClass]
public sealed partial class Straight : PatternResource
{
    [Export] public float Angle;
    [Export] public float Speed;
    [Export] public required BulletStyleResource StyleResource;
    
    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        sink.Add(Bullet.Create(
            origin,
            Vector2.Up.Rotated(Angle) * Speed,
            StyleResource.ToStyle()));
    }
}