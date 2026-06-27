using System.Collections.Generic;
using bullethell.Emitters.Patterns;
using bullethell.Entities;
using Godot;

namespace bullethell.Emitters;

public sealed class Emitter(float interval, IEmitPattern pattern)
{
    private float _timer;

    public void Update(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {
        _timer += ctx.Delta;

        // while, not if — catch up, no dropped shots at low fps      
        while (_timer >= interval)
        {
            _timer -= interval;
            pattern.Emit(origin, sink, in ctx);
        }
    }
}