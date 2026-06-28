using System.Collections;
using System.Collections.Generic;
using bullethell.Emitters;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell;

public partial class Main : Node2D
{
    private Vector2 _viewport;
    private readonly List<Bullet> _playerShots = [];
    private Boss _boss = null!;
    private Player _player = null!;
    private BulletField _field = null!;

    public override void _Ready()
    {
        _viewport = GetViewportRect().Size;
        GetViewport().SizeChanged += () =>
        {
            _viewport = GetViewportRect().Size;
            _field.PlayArea = _viewport;
        };

        _field = GetNode<BulletField>("BulletField");
        
        _player = GetNode<Player>("Player");
        _boss = GetNode<Boss>("Boss");

        _field.PlayArea = _viewport;
        _field.Target = _player;
        _field.TargetHit += Reset;
        _boss.Field = _field.Bullets;
        
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
            
            bulletEmitter.Sink = _field.Bullets;
        }
    }

    public override void _Process(double delta)
    {
        for (var i = _playerShots.Count - 1; i >= 0; i--)
        {
            var bullet = _playerShots[i];
            bullet.Position += bullet.Velocity * (float)delta;
            _playerShots[i] = bullet;
            
            if (!_boss.IsHitBy(bullet.Position, bullet.Style.HitRadius) && bullet.IsInBounds(_viewport))
                continue;

            Cull(_playerShots, i);
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
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
        _field.Bullets.Clear();
        _playerShots.Clear();
        _player.Respawn();
    }
}