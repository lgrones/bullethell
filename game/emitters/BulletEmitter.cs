using System;
using bullethell.game.bullets;
using bullethell.game.bullets.behaviors;
using bullethell.game.core.timers;
using bullethell.game.patterns;
using Godot;

namespace bullethell.game.emitters;

[Tool]
public sealed partial class BulletEmitter : Node2D
{
    [Export] public Node2D? Target;
    [Export] public Pattern? Pattern;

    [Export]
    public cadences.Cadence? Cadence
    {
        get => _cadence;
        // Setter to update the timer in real time if changed in the editor
        set
        {
            _cadence = value;
            _timer?.SetInterval(value?.NextInterval(_shotIndex, 0) ?? 0);
        }
    }

    private cadences.Cadence? _cadence;
    private IBulletSink? _sink;
    private BehaviorTable? _table;
    private IntervalTimer? _timer;
    private int _shotIndex;

    public override void _Ready()
        => _timer = new IntervalTimer(Cadence?.NextInterval(_shotIndex, 0) ?? 0);

    public override void _PhysicsProcess(double delta)
    {
        if (Pattern is null || Cadence is null || _sink is null || _table is null)
        {
            if (Engine.IsEditorHint()) return;

            throw new InvalidOperationException(
                $"Emitter not wired. Call {nameof(Initialize)} and assign a {nameof(Pattern)} and {nameof(Cadence)}");
        }

        var ticks = _timer!.Update((float)delta);
        if (ticks == 0) return;

        var ctx = new EmitContext(
            _sink,
            _table,
            Target?.GlobalPosition ?? Vector2.Zero,
            Target is not null,
            _timer.Elapsed);

        while (ticks-- > 0)
        {
            Pattern.Emit(ctx, GlobalTransform);
            _timer.AddTime(Cadence.NextInterval(_shotIndex, _timer.Elapsed));
            _shotIndex++;
        }
    }

    public void Initialize(IBulletSink sink, BehaviorTable table)
        => (_sink, _table) = (sink, table);

    public void Disable()
        => ProcessMode = ProcessModeEnum.Disabled;

    public void Enable()
        => ProcessMode = ProcessModeEnum.Inherit;
}