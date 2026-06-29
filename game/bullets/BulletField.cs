using System.Collections.Generic;
using System.Runtime.InteropServices;
using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets;

[Tool]
public sealed partial class BulletField : Node2D, IBulletSink
{
    [Export] public int MaxBullets = 4096;
    [Export] public Node2D? Player;
    [Export] private MultiMeshInstance2D _multiMeshInstance = null!;

    public BehaviorTable Table = new();

    private readonly List<Bullet> _pool = [];
    private readonly List<BulletSpawn> _pending = [];
    private MultiMesh _mesh = null!;
    private Rect2 _bounds;

    public override void _Ready()
    {
        _mesh = _multiMeshInstance.Multimesh;
        _mesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform2D;
        _mesh.InstanceCount = MaxBullets;
        _mesh.VisibleInstanceCount = 0;

        var viewport = GetViewportRect().Size;
        _bounds = new Rect2(-64, -64, viewport.X + 128, viewport.Y + 128);
    }

    public override void _PhysicsProcess(double delta)
    {
        Flush();
        UpdateBullets((float)delta, Player?.GlobalPosition ?? Vector2.Zero, Player is not null);
        Flush();
        Compact();
        SyncMultiMesh();
    }

    public override void _Draw()
    {
        if (Engine.IsEditorHint())
            DrawRect(_bounds, Colors.Red, filled: false);
    }

    public void SpawnBullet(in BulletSpawn bullet) => _pending.Add(bullet);

    private void Flush()
    {
        foreach (var s in _pending)
            _pool.Add(new Bullet
            {
                Position = s.Position,
                Velocity = s.Velocity,
                BehaviorId = s.BehaviorId,
                StateIndex = 0,
                StateTimer = 0f,
                Alive = true
            });

        _pending.Clear();
    }

    private void UpdateBullets(float dt, Vector2 targetPos, bool hasTarget)
    {
        var span = CollectionsMarshal.AsSpan(_pool);
        var defs = Table.Defs;

        for (var i = 0; i < span.Length; i++)
        {
            ref var b = ref span[i];
            ref readonly var def = ref defs[Table.Offset(b.BehaviorId) + b.StateIndex];

            b.StateTimer += dt;

            switch (def.Move)
            {
                case MovementKind.Straight:
                    b.Position += b.Velocity * dt;
                    break;

                case MovementKind.Homing:
                    if (hasTarget)
                    {
                        var desired = (targetPos - b.Position).Normalized();
                        var turn = Mathf.Clamp(b.Velocity.AngleTo(desired), -def.TurnRate * dt, def.TurnRate * dt);
                        b.Velocity = b.Velocity.Rotated(turn);
                    }

                    b.Position += b.Velocity * dt;
                    break;
            }
            
            if (!_bounds.HasPoint(b.Position))
                b.Alive = false;

            if (!ShouldExit(def, b, targetPos, hasTarget)) continue;

            var count = Table.Count(b.BehaviorId);
            if (b.StateIndex + 1 < count)
            {
                b.StateIndex++; // advance to next state
                b.StateTimer = 0f;
            }
            else // last state ended → die (maybe explode)
            {
                if (def.OnEnd == DeathAction.Explode)
                    EnqueueExplosion(b.Position, def);

                b.Alive = false;
            }
        }
    }

    private static bool ShouldExit(in StateDefinition def, in Bullet b, Vector2 target, bool hasTarget) =>
        def.Exit switch
        {
            TransitionKind.AfterTime => b.StateTimer >= def.ExitThreshold,
            TransitionKind.NearTarget => hasTarget && b.Position.DistanceTo(target) <= def.ExitThreshold,
            _ => false,
        };

    private void EnqueueExplosion(Vector2 at, in StateDefinition def)
    {
        for (var i = 0; i < def.ExplodeCount; i++)
        {
            var dir = Vector2.FromAngle(Mathf.Tau * i / def.ExplodeCount);

            _pending.Add(new BulletSpawn
            {
                Position = at,
                Velocity = dir * def.ExplodeSpeed
            });
        }
    }

    private void Compact()
    {
        for (var i = _pool.Count - 1; i >= 0; i--)
        {
            if (_pool[i].Alive) continue;

            _pool[i] = _pool[^1];
            _pool.RemoveAt(_pool.Count - 1);
        }
    }

    private void SyncMultiMesh()
    {
        _mesh.VisibleInstanceCount = _pool.Count;

        var span = CollectionsMarshal.AsSpan(_pool);

        for (var i = 0; i < span.Length; i++)
            _mesh.SetInstanceTransform2D(i, new Transform2D(0f, span[i].Position));
    }
}