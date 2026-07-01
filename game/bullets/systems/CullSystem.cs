namespace bullethell.game.bullets.systems;

/// Marks bullets outside the play bounds as dead. Compact removes them.
public sealed class CullSystem : IBulletSystem
{
    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;

        for (var i = 0; i < span.Length; i++)
        {
            ref var bullet = ref span[i];

            if (!frame.Bounds.HasPoint(bullet.Position))
                bullet.Alive = false;
        }
    }
}
