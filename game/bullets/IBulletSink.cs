namespace bullethell.game.bullets;

public interface IBulletSink
{
    void SpawnBullet(in BulletSpawn bullet);
}