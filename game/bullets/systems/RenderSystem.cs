using System;
using System.Collections.Generic;
using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets.systems;

/// Draws live bullets with one MultiMeshInstance2D per style. Each mesh carries
/// the style's Texture2D, so a style == a draw call (styles are few). Tint and
/// rainbow ride the per-instance COLOR channel, which the default 2D canvas
/// pipeline multiplies over the texture — no shader, no atlas region math.
public sealed class RenderSystem : IBulletSystem
{
    private readonly Node2D _root;
    private readonly StyleTable _styles;
    private readonly int _maxBullets;
    private readonly List<MultiMeshInstance2D> _meshes = [];
    private int[] _counts = [];

    public RenderSystem(Node2D root, StyleTable styles, int maxBullets)
    {
        _root = root;
        _styles = styles;
        _maxBullets = maxBullets;
    }

    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;
        var styles = _styles.Styles;

        EnsureMeshes(styles);
        Array.Clear(_counts, 0, _counts.Length); // per-style visible count, refilled below

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

            // hue from travel direction -> stable per bullet, rainbow around a ring
            var tint = style.Rainbow
                ? Color.FromHsv(Mathf.PosMod(b.Velocity.Angle() / Mathf.Tau + frame.Time * 0.15f, 1f), 1f, 1f)
                : style.Tint;

            var mesh = _meshes[b.StyleId].Multimesh;
            var slot = _counts[b.StyleId]++;
            mesh.SetInstanceTransform2D(slot, t);
            mesh.SetInstanceColor(slot, tint);
        }

        for (var s = 0; s < _meshes.Count; s++)
            _meshes[s].Multimesh.VisibleInstanceCount = _counts[s];
    }

    /// Spin up a MultiMeshInstance2D for each newly registered style. Runs only
    /// when a style first appears, so no per-frame allocation. Children are added
    /// without an owner, so the [Tool] preview never serializes them into the scene.
    private void EnsureMeshes(ReadOnlySpan<BulletStyle> styles)
    {
        while (_meshes.Count < styles.Length)
        {
            var style = styles[_meshes.Count];
            var node = new MultiMeshInstance2D
            {
                Texture = Resolve(style.Texture),
                Material = style.Glow
                    ? new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add }
                    : null,
                Multimesh = new MultiMesh
                {
                    TransformFormat = MultiMesh.TransformFormatEnum.Transform2D,
                    UseColors = true,
                    Mesh = new QuadMesh { Size = Vector2.One },
                    InstanceCount = _maxBullets,
                    VisibleInstanceCount = 0,
                },
            };
            _root.AddChild(node);
            _meshes.Add(node);
        }

        if (_counts.Length < _meshes.Count)
            _counts = new int[_meshes.Count];
    }

    /// A MultiMesh samples its texture with the mesh's raw 0..1 UVs, which ignore
    /// an AtlasTexture's region (only Sprite2D/TextureRect honor that). So bake the
    /// region into a standalone ImageTexture, else the whole sheet garbles the quad.
    private static Texture2D? Resolve(Texture2D? texture)
    {
        if (texture is not AtlasTexture atlas || atlas.Atlas is null)
            return texture;

        var crop = atlas.Atlas.GetImage().GetRegion((Rect2I)atlas.Region);
        return ImageTexture.CreateFromImage(crop);
    }
}
