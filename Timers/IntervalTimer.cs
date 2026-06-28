using System;

namespace bullethell.Timers;

public class IntervalTimer(float interval, int maxTicks = 8)
{
    private float _elapsed;

    public int Update(in FrameContext ctx)
    {
        _elapsed += ctx.Delta;

        // while, not if — catch up, no dropped shots at low fps      
        var ticks = 0;                                                                                                                                            
        while (_elapsed >= interval)                                                                                                                              
        {                                                                                                                                                         
            _elapsed -= interval;                                                                                                                                 
            ticks++;                                                                                                                                              
        }  
        
        return Math.Min(ticks, maxTicks);   
    }
}