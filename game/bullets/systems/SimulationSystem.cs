using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.bullets.systems;

/// Movement, per-bullet state machine, and death actions (explosions).
public sealed class SimulationSystem(BehaviorTable table) : IBulletSystem
{
    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var bullets = pool.Active;
        var states = table.Defs;

        for (var i = 0; i < bullets.Length; i++)
        {
            ref var bullet = ref bullets[i];
            ref readonly var state = ref states[table.Offset(bullet.BehaviorId) + bullet.StateIndex];

            bullet.StateTimer += frame.Dt;

            switch (state.Move)
            {
                case MovementKind.Straight:
                    bullet.Position += bullet.Velocity * frame.Dt;
                    break;

                case MovementKind.Homing:
                    if (frame.HasTarget)
                    {
                        var desiredDirection = (frame.Target - bullet.Position).Normalized();
                        var turnAngle = Mathf.Clamp(
                            bullet.Velocity.AngleTo(desiredDirection),
                            -state.TurnRate * frame.Dt,
                            state.TurnRate * frame.Dt);

                        bullet.Velocity = bullet.Velocity.Rotated(turnAngle);
                    }

                    bullet.Position += bullet.Velocity * frame.Dt;
                    break;
            }

            if (!ShouldExit(state, bullet, frame))
                continue;

            var stateCount = table.Count(bullet.BehaviorId);

            if (bullet.StateIndex + 1 < stateCount)
            {
                bullet.StateIndex++;
                bullet.StateTimer = 0f;
            }
            else // last state ended → die (maybe explode)
            {
                if (state.OnEnd == DeathAction.Explode)
                    Explode(pool, bullet.Position, bullet.StyleId, state);

                bullet.Alive = false;
            }
        }
    }

    private static bool ShouldExit(in StateDefinition state, in Bullet bullet, in BulletFrame frame) =>
        state.Exit switch
        {
            TransitionKind.AfterTime => bullet.StateTimer >= state.ExitThreshold,
            TransitionKind.NearTarget => frame.HasTarget && bullet.Position.DistanceTo(frame.Target) <= state.ExitThreshold,
            _ => false,
        };

    private static void Explode(BulletPool pool, Vector2 position, byte styleId, in StateDefinition state)
    {
        for (var shard = 0; shard < state.ExplodeCount; shard++)
        {
            var direction = Vector2.FromAngle(Mathf.Tau * shard / state.ExplodeCount);

            pool.SpawnBullet(new BulletSpawn
            {
                Position = position,
                Velocity = direction * state.ExplodeSpeed,
                StyleId = styleId
            });
        }
    }
}
