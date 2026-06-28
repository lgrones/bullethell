using System.Collections;
using System.Collections.Generic;
using bullethell.Emitters;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050
public partial class Main : Node2D
#pragma warning restore CA1050
{
    private Vector2 _viewport;
    private readonly List<Bullet> _bullets = [];
    private readonly List<Bullet> _playerShots = [];
    private Player _player = null!;
    private Boss _boss = null!;
    private MultiMesh _bulletMesh = null!;

    public override void _Ready()
    {
        _viewport = GetViewportRect().Size;
        GetViewport().SizeChanged += () => _viewport = GetViewportRect().Size;

        _bulletMesh = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform2D,
            Mesh = new QuadMesh { Size = new Vector2(20, 20) }
        };

        GetNode<MultiMeshInstance2D>("BulletRenderer").Multimesh = _bulletMesh;
        
        _player = GetNode<Player>("Player");
        _boss = GetNode<Boss>("Boss");

        _boss.Sink = _bullets;
        
        foreach (var emitter in GetTree().GetNodesInGroup("playerEmitters"))
        {
            if (emitter is not BulletEmitter bulletEmitter) 
                continue;
            
            bulletEmitter.Disable();
            bulletEmitter.Sink = _playerShots;
            _player.Emitters.Add(bulletEmitter);
        }
        
        foreach (var emitter in GetTree().GetNodesInGroup("bossEmitters"))
        {
            if (emitter is not BulletEmitter bulletEmitter) 
                continue;
            
            bulletEmitter.Sink = _bullets;
        }
    }

    public override void _Process(double delta)
    {
        for (var i = _bullets.Count - 1; i >= 0; i--)
        {
            var bullet = _bullets[i];
            bullet.Position += bullet.Velocity * (float)delta;
            _bullets[i] = bullet;

            var isHit = _player.IsHit(bullet.Position, bullet.Style.HitRadius);

            if (!isHit && bullet.IsInBounds(_viewport))
                continue;

            if (isHit)
            {
                Reset();
                break;
            }

            Cull(_bullets, i);
        }

        for (var i = _playerShots.Count - 1; i >= 0; i--)
        {
            var bullet = _playerShots[i];
            bullet.Position += bullet.Velocity * (float)delta;
            _playerShots[i] = bullet;

            var isHit = _boss.CheckHit(bullet.Position, bullet.Style.HitRadius);

            if (!isHit && bullet.IsInBounds(_viewport))
                continue;

            Cull(_playerShots, i);
        }

        _bulletMesh.InstanceCount = _bullets.Count;

        for (var i = 0; i < _bullets.Count; i++)
        {
            _bulletMesh.SetInstanceTransform2D(i, new Transform2D(0f, _bullets[i].Position));
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(_player.Position, _player.Radius, Colors.White);

        if (Player.IsFocused())
            DrawCircle(_player.Position, _player.HitRadius, Colors.Red);

        DrawCircle(_boss.Position, _boss.Radius, Colors.GreenYellow);

        foreach (var playerShot in _playerShots)
        {
            DrawCircle(playerShot.Position, playerShot.Style.Radius, Colors.White);
        }
    }

    private static void Cull(IList sink, int index)
    {
        sink[index] = sink[^1];
        sink.RemoveAt(sink.Count - 1);
    }

    private void Reset()
    {
        _bullets.Clear();
        _playerShots.Clear();
        _player.Respawn();
    }
}