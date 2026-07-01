using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets.systems;

/// Movement, per-bullet state machine, and death actions (explosions).
public sealed class SimulationSystem(BehaviorTable table) : IBulletSystem
{
    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;
        var defs = table.Defs;

        for (var i = 0; i < span.Length; i++)
        {
            ref var b = ref span[i];
            ref readonly var def = ref defs[table.Offset(b.BehaviorId) + b.StateIndex];

            b.StateTimer += frame.Dt;

            switch (def.Move)
            {
                case MovementKind.Straight:
                    b.Position += b.Velocity * frame.Dt;
                    break;

                case MovementKind.Homing:
                    if (frame.HasTarget)
                    {
                        var desired = (frame.Target - b.Position).Normalized();
                        var turn = Mathf.Clamp(b.Velocity.AngleTo(desired), -def.TurnRate * frame.Dt, def.TurnRate * frame.Dt);
                        b.Velocity = b.Velocity.Rotated(turn);
                    }

                    b.Position += b.Velocity * frame.Dt;
                    break;
            }

            if (!ShouldExit(def, b, frame)) continue;

            var count = table.Count(b.BehaviorId);
            if (b.StateIndex + 1 < count)
            {
                b.StateIndex++; // advance to next state
                b.StateTimer = 0f;
            }
            else // last state ended → die (maybe explode)
            {
                if (def.OnEnd == DeathAction.Explode)
                    Explode(pool, b.Position, b.StyleId, def);

                b.Alive = false;
            }
        }
    }

    private static bool ShouldExit(in StateDefinition def, in Bullet b, in BulletFrame frame) =>
        def.Exit switch
        {
            TransitionKind.AfterTime => b.StateTimer >= def.ExitThreshold,
            TransitionKind.NearTarget => frame.HasTarget && b.Position.DistanceTo(frame.Target) <= def.ExitThreshold,
            _ => false,
        };

    private static void Explode(BulletPool pool, Vector2 at, byte styleId, in StateDefinition def)
    {
        for (var i = 0; i < def.ExplodeCount; i++)
        {
            var dir = Vector2.FromAngle(Mathf.Tau * i / def.ExplodeCount);

            pool.SpawnBullet(new BulletSpawn
            {
                Position = at,
                Velocity = dir * def.ExplodeSpeed,
                StyleId = styleId
            });
        }
    }
}
