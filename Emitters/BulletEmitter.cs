using System;
using System.Collections.Generic;
using bullethell.Emitters.Patterns;
using bullethell.Entities.Bullets;
using bullethell.Timers;
using Godot;
using PatternResource = bullethell.Emitters.Resources.PatternResource;

namespace bullethell.Emitters;

public sealed partial class BulletEmitter : Node2D
{
    [Export] public float Interval;
    [Export] public PatternResource? Pattern;
    [Export] public Node2D? Target;

    public List<Bullet>? Sink;
    private bool _enabled = true;

    private IntervalTimer _timer = null!;

    public override void _Ready()
    {
        _timer = new IntervalTimer(Interval);
    }

    public override void _Process(double delta)
    {
        if (!_enabled)
            return;

        if (Sink is null)
            throw new InvalidOperationException($"{nameof(BulletEmitter)} requires {nameof(Sink)} before processing.");

        if (Pattern is null)
            throw new InvalidOperationException(
                $"{nameof(BulletEmitter)} requires {nameof(Pattern)} before processing.");

        var ticks = _timer.Update((float)delta);

        while (ticks-- > 0)
            Pattern.Emit(GlobalPosition, Sink, Target?.GlobalPosition);
    }

    public void Enable()
        => _enabled = true;
    
    public void Disable()
        => _enabled = false;
}