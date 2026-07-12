using System;
using System.Collections.Generic;
using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets.systems;

/// Draws live bullets with one MultiMeshInstance2D per style. Each mesh carries
/// the style's Texture2D and optional Material, so a style == a draw call
/// (stylesTable are few). Shading is whatever ShaderMaterial the style assigns.
public sealed class RenderSystem(Node2D root, StyleTable stylesTable, int maxBullets) : IBulletSystem
{
    private readonly List<MultiMeshInstance2D> _meshes = [];
    private int[] _counts = [];

    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;
        var styles = stylesTable.Styles;

        EnsureMeshes(styles);
        Array.Clear(_counts, 0, _counts.Length); // per-style visible count, refilled below

        for (var i = 0; i < span.Length; i++)
        {
            ref readonly var bullet = ref span[i];
            ref readonly var style = ref styles[bullet.StyleId];

            // scale unit quad to the style radius, oriented per rotation mode
            var angle = style.Rotation switch
            {
                RotationMode.AlignVelocity => bullet.Velocity.Angle(),
                RotationMode.Spin => frame.Time * style.SpinRate,
                _ => 0f, // Fixed
            };
            
            var transform = new Transform2D(angle, bullet.Position);
            transform.X *= style.Radius;
            transform.Y *= style.Radius;

            var mesh = _meshes[bullet.StyleId].Multimesh;
            var slot = _counts[bullet.StyleId]++;
            mesh.SetInstanceTransform2D(slot, transform);
        }

        for (var styleIndex = 0; styleIndex < _meshes.Count; styleIndex++)
            _meshes[styleIndex].Multimesh.VisibleInstanceCount = _counts[styleIndex];
    }

    /// Spin up a MultiMeshInstance2D for each newly registered style. Runs only
    /// when a style first appears, so no per-frame allocation. Children are added
    /// without an owner, so the [Tool] preview never serializes them into the scene.
    private void EnsureMeshes(ReadOnlySpan<BulletStyle> styles)
    {
        while (_meshes.Count < styles.Length)
        {
            var style = styles[_meshes.Count];
            var meshInstance = new MultiMeshInstance2D
            {
                Texture = Resolve(style.Texture),
                Material = style.Material,
                Multimesh = new MultiMesh
                {
                    TransformFormat = MultiMesh.TransformFormatEnum.Transform2D,
                    Mesh = new QuadMesh { Size = Vector2.One },
                    InstanceCount = maxBullets,
                    VisibleInstanceCount = 0,
                },
            };

            root.AddChild(meshInstance);
            _meshes.Add(meshInstance);
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
