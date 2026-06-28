using System;
using System.Collections.Generic;
using bullethell.Emitters.Resources;
using bullethell.Entities.Bullets;
using bullethell.Timers;
using Godot;

namespace bullethell.Emitters;

[Tool]
public sealed partial class BulletEmitter : Node2D
{
    [Export] public PatternResource? Pattern;
    [Export] public Node2D? Target;

    [Export]
    public float Interval
    {
        get => _interval;
        set
        {
            _interval = value;
            _timer?.SetInterval(value);
        }
    }

    public List<Bullet>? Sink;
    private float _interval;
    private bool _enabled = true;

    private IntervalTimer? _timer;

    public override void _Ready()
    {
        _timer = new IntervalTimer(Interval);
    }

    public override void _Process(double delta)
    {
        if (!_enabled)
            return;

        if (Pattern is null || Sink is null)
        {
            if (Engine.IsEditorHint()) return;
            throw new InvalidOperationException("Emitter not wired");
        }

        var ticks = _timer?.Update((float)delta);

        while (ticks-- > 0)
            Pattern.Emit(GlobalPosition, Sink, Target?.GlobalPosition);
    }

    public void Enable()
        => _enabled = true;

    public void Disable()
        => _enabled = false;
}