using System.Collections.Generic;
using bullethell.Entities;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

public sealed class Composite(params IEmitPattern[] inner) : IEmitPattern
{
    public void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        foreach (var pattern in inner)
            pattern.Emit(origin, sink, in ctx);
    }
}