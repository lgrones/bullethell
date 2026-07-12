using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bullethell.game.bullets;

/// Owns bullet storage. Spawns are buffered then admitted on Flush;
/// dead bullets are removed on Compact. Systems operate on Active.
public sealed class BulletPool(int capacity) : IBulletSink
{
    private readonly List<Bullet> _pool = new(capacity);
    private readonly List<BulletSpawn> _pending = [];

    public int Count => _pool.Count;
    public Span<Bullet> Active => CollectionsMarshal.AsSpan(_pool);

    public void SpawnBullet(in BulletSpawn spawn) => _pending.Add(spawn);

    public void Flush()
    {
        foreach (var spawn in _pending)
            _pool.Add(new Bullet
            {
                Position = spawn.Position,
                Velocity = spawn.Velocity,
                BehaviorId = spawn.BehaviorId,
                StyleId = spawn.StyleId,
                StateIndex = 0,
                StateTimer = 0f,
                Alive = true,
            });

        _pending.Clear();
    }

    public void Compact()
    {
        for (var i = _pool.Count - 1; i >= 0; i--)
        {
            if (_pool[i].Alive) continue;

            _pool[i] = _pool[^1];
            _pool.RemoveAt(_pool.Count - 1);
        }
    }

    public void Clear()
    {
        _pool.Clear();
        _pending.Clear();
    }
}
