using System.Collections.Generic;
using bullethell.Emitters.Patterns;
using bullethell.Entities.Bullets;
using bullethell.Timers;
using Godot;

namespace bullethell.Emitters;

public sealed class BulletEmitter(float interval, IEmitPattern pattern)
{
    private readonly IntervalTimer _timer = new(interval);

    public void Update(Vector2 origin, List<Bullet> sink, in FrameContext ctx)
    {                                                                                                                                                                 
        var ticks = _timer.Update(in ctx);      
        
        while(ticks-- > 0)                                                                                                                             
            pattern.Emit(origin, sink, in ctx);                                                                                                                       
    }  
}