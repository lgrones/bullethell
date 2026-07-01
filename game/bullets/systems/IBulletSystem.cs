namespace bullethell.game.bullets.systems;

/// One responsibility over the bullet pool for a frame. Composed into a
/// pipeline by BulletField; add collisions, scripting, etc. as more systems.
public interface IBulletSystem
{
    void Run(BulletPool pool, in BulletFrame frame);
}
