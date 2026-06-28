using System.Collections.Generic;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[GlobalClass]
public sealed partial class Straight : Resources.PatternResource
{
    [Export] public float Angle;
    [Export] public float Speed;
    [Export] public required Resources.BulletStyleResource StyleResource;
    
    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        sink.Add(Bullet.Create(
            origin,
            Vector2.Up.Rotated(Angle) * Speed,
            StyleResource.ToStyle()));
    }
}