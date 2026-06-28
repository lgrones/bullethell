using System.Collections.Generic;
using bullethell.Emitters.Resources;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[Tool]
[GlobalClass]
public sealed partial class Spiral : PatternResource
{
    [Export] public float StartAngle;
    [Export] public float Turn;
    [Export] public float Speed;
    [Export] public required BulletStyleResource StyleResource;

    private float? _angle;

    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        _angle ??= StartAngle;
        _angle += Turn;
        
        sink.Add(Bullet.Create(
            origin,
            Vector2.Right.Rotated(_angle.Value) * Speed,
            StyleResource.ToStyle()));
    }
}