using System.Collections.Generic;
using bullethell.game.bullets;
using bullethell.game.bullets.behaviors;
using bullethell.game.core.timers;
using bullethell.game.emitters.cadences;
using bullethell.game.patterns;
using Godot;

namespace bullethell.game.emitters;

[Tool]
public sealed partial class BulletEmitter : Node2D
{
    [Export] public Node2D? Target;

    [Export]
    public Pattern? Pattern
    {
        get => _pattern;
        set
        {
            _pattern = value;
            UpdateConfigurationWarnings();
        }
    }

    [Export]
    public Cadence? Cadence
    {
        get => _cadence;
        set
        {
            _cadence = value;
            UpdateConfigurationWarnings();
        }
    }

    private Pattern? _pattern;
    private Cadence? _cadence;
    private IBulletSink? _sink;
    private BehaviorTable? _table;
    private IntervalTimer? _timer;
    private int _shotIndex;

    // Start Due so the first physics tick pulls the real interval from Cadence.
    public override void _Ready()
        => _timer = new IntervalTimer(0f);

    public override void _PhysicsProcess(double delta)
    {
        if (Pattern is null || Cadence is null || _sink is null || _table is null)
            return;

        _timer!.Advance((float)delta);
        if (!_timer.Due) return;

        var ctx = new EmitContext(
            _sink,
            _table,
            Target?.GlobalPosition ?? Vector2.Zero,
            Target is not null,
            _timer.Elapsed);

        var ticks = 0;
        while (_timer.Due && ticks < _timer.MaxTicks)
        {
            Pattern.Emit(ctx, GlobalTransform);
            _timer.AddTime(Cadence.NextInterval(_shotIndex, _timer.Elapsed));
            _shotIndex++;
            ticks++;
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (Pattern is null)
            warnings.Add($"Assign a {nameof(Pattern)}.");

        if (Cadence is null)
            warnings.Add($"Assign a {nameof(Cadence)}.");

        return [.. warnings];
    }

    public void Initialize(IBulletSink sink, BehaviorTable table)
        => (_sink, _table) = (sink, table);

    public void Disable()
        => ProcessMode = ProcessModeEnum.Disabled;

    public void Enable()
        => ProcessMode = ProcessModeEnum.Inherit;
}