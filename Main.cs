using System.Collections.Generic;
using bullethell;
using bullethell.Entities;
using Godot;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050
public partial class Main : Node2D
#pragma warning restore CA1050
{
	private Vector2 _viewport;
	private readonly Player _player = new();
	private readonly Boss _boss = new();
	private readonly List<Bullet> _bullets = [];
	private MultiMesh _bulletMesh = null!;

	public override void _Ready()
	{
		_bulletMesh = new MultiMesh
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform2D,
			Mesh = new QuadMesh { Size = new Vector2(20, 20) }
		};
		
		GetNode<MultiMeshInstance2D>("BulletRenderer").Multimesh = _bulletMesh;

		_viewport = GetViewportRect().Size;
		GetViewport().SizeChanged += () => _viewport = GetViewportRect().Size;

		_player.Spawn(_viewport);
		_boss.Spawn(_viewport);
	}

	public override void _Process(double delta)
	{
		var ctx = new FrameContext
		{
			Viewport = _viewport,
			Delta = (float)delta,
			PlayerPosition = _player.Position
		};

		_player.Move(in ctx);
		ctx = ctx with { PlayerPosition = _player.Position };

		_boss.Fire(_bullets, in ctx);

		for (var i = _bullets.Count - 1; i >= 0; i--)
		{
			var bullet = _bullets[i];
			bullet.Position += bullet.Velocity * (float)delta;
			_bullets[i] = bullet;

			if (_player.IsHit(bullet.Position, bullet.Style.HitRadius))
			{
				_bullets.Clear();
				_player.Spawn(_viewport);
				break;
			}

			if (bullet.IsInBounds(_viewport))
				continue;

			_bullets[i] = _bullets[^1];
			_bullets.RemoveAt(_bullets.Count - 1);
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
		DrawCircle(_player.Position, Player.Radius, Colors.White);

		if (Player.IsFocused())
			DrawCircle(_player.Position, Player.HitRadius, Colors.Red);

		DrawCircle(_boss.Position, Boss.Radius, Colors.GreenYellow);
	}
}
