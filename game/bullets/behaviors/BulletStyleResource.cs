using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletStyleResource : Resource
{
    [Export] public float Radius = 8f;
    [Export] public float HitRadius = 4f;

    /// Tint multiplied over the sprite. Ignored when Rainbow is on.
    [Export] public Color Color = Colors.White;

    /// Cycle hue per bullet at draw time (Marisa rainbow stars). Use a pale/white region.
    [Export] public bool Rainbow;

    /// Source rect in the atlas, in pixels. Normalized to UV by the field.
    [Export] public Rect2 Region = new(0, 0, 16, 16);

    /// How the sprite is oriented each frame.
    [Export] public RotationMode Rotation = RotationMode.AlignVelocity;

    /// Radians per second when Rotation is Spin.
    [Export] public float SpinRate = 6f;
}