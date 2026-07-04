using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

namespace bullethell.game.bullets.behaviors;

/// How a bullet sprite is oriented each frame.
public enum RotationMode : byte
{
    AlignVelocity, // face travel direction (kunai, rice, arrows)
    Fixed,         // never rotate (orbs, upright stars)
    Spin,          // continuous spin at SpinRate rad/s
}

/// Baked, render-ready copy of a BulletStyleResource. One MultiMesh per style,
/// so the texture rides along and picks the draw call.
public struct BulletStyle
{
    public Texture2D? Texture;
    public Color Tint;
    public bool Rainbow;
    public bool Glow;
    public float Radius;
    public float HitRadius;
    public RotationMode Rotation;
    public float SpinRate;
}

/// Interns BulletStyleResources into byte ids, parallel to BehaviorTable.
public sealed class StyleTable
{
    private readonly List<BulletStyle> _styles = [];
    private readonly Dictionary<BulletStyleResource, byte> _ids = new();

    public ReadOnlySpan<BulletStyle> Styles => CollectionsMarshal.AsSpan(_styles);

    public StyleTable()
    {
        // id 0 = fallback: no texture -> white quad, plain white.
        _styles.Add(new BulletStyle
        {
            Texture = null,
            Tint = Colors.White,
            Radius = 8f,
            HitRadius = 4f,
            Rotation = RotationMode.AlignVelocity,
        });
    }

    public byte IdFor(BulletStyleResource? style)
    {
        if (style is null) return 0;
        if (_ids.TryGetValue(style, out var id)) return id;

        return Register(style);
    }

    private byte Register(BulletStyleResource style)
    {
        var id = (byte)_styles.Count;

        _ids[style] = id;
        _styles.Add(new BulletStyle
        {
            Texture = style.Texture,
            Tint = style.Color,
            Rainbow = style.Rainbow,
            Glow = style.Glow,
            Radius = style.Radius,
            HitRadius = style.HitRadius,
            Rotation = style.Rotation,
            SpinRate = style.SpinRate,
        });

        return id;
    }
}
