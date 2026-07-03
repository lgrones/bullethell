using bullethell.game.bullets.behaviors;

namespace bullethell.game.bullets.systems;

/// Marks bullets outside the play bounds as dead. Compact removes them.
/// Bounds are grown by the sprite radius so a bullet lives until it's fully
/// past the edge — culling on the bare center pops the sprite while half visible.
public sealed class CullSystem : IBulletSystem
{
    private readonly StyleTable _styles;

    public CullSystem(StyleTable styles) => _styles = styles;

    public void Run(BulletPool pool, in BulletFrame frame)
    {
        var span = pool.Active;
        var styles = _styles.Styles;

        for (var i = 0; i < span.Length; i++)
        {
            ref var bullet = ref span[i];
            var radius = styles[bullet.StyleId].Radius;

            if (!frame.Bounds.Grow(radius).HasPoint(bullet.Position))
                bullet.Alive = false;
        }
    }
}
