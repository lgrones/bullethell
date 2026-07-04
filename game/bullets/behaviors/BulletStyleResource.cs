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

    /// Cycle hue per bullet at draw time (Marisa rainbow stars). Use a pale/white texture.
    [Export] public bool Rainbow;

    /// Additive blend so cores blow out and overlaps bloom (classic bullet glow).
    [Export] public bool Glow = true;

    /// Sprite drawn for this bullet. AtlasTexture works too (crop a shared sheet).
    [Export] public Texture2D? Texture;

    /// How the sprite is oriented each frame.
    [Export] public RotationMode Rotation = RotationMode.AlignVelocity;

    /// Radians per second when Rotation is Spin.
    [Export] public float SpinRate = 6f;
}