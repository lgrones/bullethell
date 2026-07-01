using System;
using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets.systems;

/// Pushes live bullets into a MultiMesh. 2D MultiMesh INSTANCE_CUSTOM aliases the
/// instance color, so everything rides the COLOR channel: rgb = tint, a = style id.
/// Atlas rects live in the shader's regions[] uniform, indexed by style id.
public sealed class RenderSystem : IBulletSystem
{
    private readonly MultiMesh _mesh;
    private readonly StyleTable _styles;
    private readonly ShaderMaterial? _material;
    private readonly Vector2 _atlasSize;
    private int _pushedStyles;

    public RenderSystem(MultiMeshInstance2D instance, StyleTable styles, Texture2D? atlas, int maxBullets)
    {
        _styles = styles;
        _mesh = instance.Multimesh;
        _mesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform2D;
        _mesh.UseColors = true;        // per-instance tint + packed style id
        _mesh.UseCustomData = false;   // unusable in 2D (aliases color)
        _mesh.InstanceCount = maxBullets;
        _mesh.VisibleInstanceCount = 0;

        _material = instance.Material as ShaderMaterial;
        _atlasSize = atlas?.GetSize() ?? Vector2.One;
        if (atlas is not null)
            _material?.SetShaderParameter("atlas", atlas);
    }

    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;
        var styles = _styles.Styles;

        if (styles.Length != _pushedStyles)
            PushRegions(styles);

        _mesh.VisibleInstanceCount = span.Length;

        for (var i = 0; i < span.Length; i++)
        {
            ref readonly var b = ref span[i];
            ref readonly var style = ref styles[b.StyleId];

            // scale unit quad to the style radius, oriented per rotation mode
            var angle = style.Rotation switch
            {
                RotationMode.AlignVelocity => b.Velocity.Angle(),
                RotationMode.Spin => frame.Time * style.SpinRate,
                _ => 0f, // Fixed
            };
            var t = new Transform2D(angle, b.Position);
            t.X *= style.Radius;
            t.Y *= style.Radius;
            _mesh.SetInstanceTransform2D(i, t);

            // hue from travel direction -> stable per bullet, rainbow around a ring
            var tint = style.Rainbow
                ? Color.FromHsv(Mathf.PosMod(b.Velocity.Angle() / Mathf.Tau + frame.Time * 0.15f, 1f), 1f, 1f)
                : style.Tint;

            // rgb = tint, a = style id (index into regions[])
            _mesh.SetInstanceColor(i, new Color(tint.R, tint.G, tint.B, b.StyleId / 255f));
        }
    }

    /// Push each style's atlas rect (normalized UV) into the shader's regions[] uniform.
    /// Only runs when a new style registers, so no per-frame allocation.
    private void PushRegions(ReadOnlySpan<BulletStyle> styles)
    {
        // 256 zero-padded entries to match `uniform vec4 regions[256]`.
        var regions = new Godot.Collections.Array<Vector4>();

        for (var i = 0; i < 256; i++)
        {
            if (i >= styles.Length)
            {
                regions.Add(Vector4.Zero);
                continue;
            }

            var reg = styles[i].Region;
            regions.Add(new Vector4(
                reg.Position.X / _atlasSize.X,
                reg.Position.Y / _atlasSize.Y,
                reg.Size.X / _atlasSize.X,
                reg.Size.Y / _atlasSize.Y));
        }

        _material?.SetShaderParameter("regions", regions);
        _pushedStyles = styles.Length;
    }
}
