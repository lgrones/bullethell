using System.Collections.Generic;
using bullethell.Entities;
using Godot;

namespace bullethell.Emitters.Patterns;

// Decorator: rotate the inner pattern's whole shot so it points at the player.
// Fuses with any pattern -> aimed spiral, aimed ring, etc. (vs Composite, which overlays).
public sealed class AimedAt(IEmitPattern inner) : IEmitPattern
{
    private readonly List<Bullet> _tmp = [];

    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        _tmp.Clear();
        inner.Emit(origin, _tmp, in ctx);

        var aim = (ctx.PlayerPosition - origin).Angle();
        foreach (var bullet in _tmp)
            sink.Add(bullet with { Velocity = bullet.Velocity.Rotated(aim) });
    }
}
