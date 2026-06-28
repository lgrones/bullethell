using System;
using System.Collections.Generic;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell;

[Tool]
public partial class BulletField : Node2D
{
    [Export] public Vector2 PlayArea;
    
    public readonly List<Bullet> Bullets = [];
    public ICollidable? Target;
    public event Action? TargetHit;

    private MultiMesh _bulletMesh = null!;

    public override void _Ready()
    {
        _bulletMesh = GetNode<MultiMeshInstance2D>("BulletRenderer").Multimesh;
    }

    public override void _Process(double delta)
    {
        var targetPosition = Target?.Position;
        var targetHitRadius = Target?.HitRadius;

        for (var i = Bullets.Count - 1; i >= 0; i--)
        {
            var bullet = Bullets[i];
            bullet.Position += bullet.Velocity * (float)delta;
            Bullets[i] = bullet;

            if (!bullet.IsInBounds(PlayArea))
            {
                Cull(i);
                continue;
            }

            if (Target is null)
                continue;

            var isHit = ICollidable.Overlaps(
                targetPosition!.Value,
                targetHitRadius!.Value,
                bullet.Position,
                bullet.Style.HitRadius);

            if (!isHit) continue;
            
            Cull(i);
            TargetHit?.Invoke();
            break;
        }

        _bulletMesh.InstanceCount = Bullets.Count;

        for (var i = 0; i < Bullets.Count; i++)
        {
            _bulletMesh.SetInstanceTransform2D(i, new Transform2D(0f, Bullets[i].Position));
        }
    }

    public override void _Draw()
    {
       if(Engine.IsEditorHint())
           DrawRect(new Rect2
           {
               Position = Vector2.Zero,
               Size = PlayArea
           }, Colors.Red, filled: false);
    }

    private void Cull(int index)
    {
        Bullets[index] = Bullets[^1];
        Bullets.RemoveAt(Bullets.Count - 1);
    }
}