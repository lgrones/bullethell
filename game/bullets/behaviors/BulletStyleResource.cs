using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletStyleResource : Resource
{
    [Export] public float Radius;
    [Export] public float HitRadius;
    [Export] public Color Color;

    public BulletStyle ToStyle()
        => new(Radius, HitRadius, Color);
}