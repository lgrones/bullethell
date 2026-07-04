using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletStyleResource : Resource
{
    [Export] public float Radius = 8f;
    [Export] public float HitRadius = 4f;

    /// Sprite drawn for this bullet. AtlasTexture works too (crop a shared sheet).
    [Export] public Texture2D? Texture;

    /// Shader/material drawn over the whole pattern. Assign a ShaderMaterial to
    /// tint, rainbow, glow, or animate this style however you like. Null = plain.
    [Export] public Material? Material;

    /// How the sprite is oriented each frame.
    [Export] public RotationMode Rotation = RotationMode.AlignVelocity;

    /// Radians per second when Rotation is Spin.
    [Export] public float SpinRate = 6f;
}