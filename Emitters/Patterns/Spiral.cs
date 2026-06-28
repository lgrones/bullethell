using System.Collections.Generic;
using bullethell.Emitters.Resources;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

[GlobalClass]
public sealed partial class Spiral : PatternResource
{
    [Export] public float StartAngle;
    [Export] public float Turn;
    [Export] public float Speed;
    [Export] public required BulletStyleResource StyleResource;

    private float _angle;

    public Spiral()
        => _angle = StartAngle;

    public override void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null)
    {
        _angle += Turn;
        
        sink.Add(Bullet.Create(
            origin,
            Vector2.Right.Rotated(_angle) * Speed,
            StyleResource.ToStyle()));
    }
}