using System.Collections.Generic;
using System.Linq;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

public sealed class RepeatRotated(int arms, IEmitPattern inner) : IEmitPattern
{
    private readonly List<Bullet> _tmp = [];

    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        _tmp.Clear();
        inner.Emit(origin, _tmp, in ctx);

        for (var arm = 0; arm < arms; arm++)
        {
            var offset = Mathf.Tau * arm / arms;

            sink.AddRange(_tmp.Select(bullet => bullet with
            {
                Position = origin + (bullet.Position - origin).Rotated(offset), 
                Velocity = bullet.Velocity.Rotated(offset),
            }));
        }
    }
}