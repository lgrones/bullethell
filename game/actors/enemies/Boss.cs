using bullethell.game.bullets;
using bullethell.game.core;
using bullethell.game.emitters;
using bullethell.game.phases;
using Godot;
using Timer = bullethell.game.core.timers.Timer;

namespace bullethell.game.actors.enemies;

public partial class Boss : Node2D, ICollidable
{
    /// Raised when the last phase ends. Main turns this into a win/reset.
    [Signal]
    public delegate void DefeatedEventHandler();

    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 10f;
    [Export] public PackedScene[] Phases = [];

    /// Field the phase emitters fire into, and the player they aim at. Set by Main.
    public BulletField Field = null!;
    public Node2D? PlayerTarget;

    private int _currentPhase;
    private uint _currentHp;
    private BossPhase? _phase;
    private Timer _phaseTimer = null!;

    public override void _Ready()
    {
        var viewport = GetViewportRect().Size;
        Position = new Vector2(viewport.X * 0.5f, viewport.Y * 0.15f);
    }

    /// Kicks off phase 0. Called by Main after Field is wired (Boss._Ready runs
    /// before Main._Ready, so this can't live in _Ready).
    public void Begin()
    {
        if (Phases.Length > 0)
            EnablePhase();
    }

    public override void _Process(double delta)
    {
        if (_phase is null)
            return;

        _phaseTimer.Update((float)delta);

        if (_currentHp == 0 || _phaseTimer.IsElapsed)
            NextPhase();
    }

    public override void _Draw()
        => DrawCircle(Vector2.Zero, Radius, Colors.GreenYellow);

    /// One bullet connected. Drains phase HP; _Process advances when it empties.
    public void Hit()
    {
        if (_currentHp > 0)
            _currentHp--;
    }

    private void NextPhase()
    {
        _phase?.QueueFree();
        _phase = null;
        Field.Clear();

        _currentPhase++;

        if (_currentPhase >= Phases.Length)
        {
            EmitSignal(SignalName.Defeated);
            return;
        }

        EnablePhase();
    }

    private void EnablePhase()
    {
        var phase = Phases[_currentPhase].Instantiate<BossPhase>();
        AddChild(phase);

        foreach (var emitter in phase.FindEmitters())
        {
            emitter.Target = PlayerTarget;
            emitter.Initialize(Field, Field.Table, Field.Styles);
        }

        _phase = phase;
        _phaseTimer = new Timer(phase.Duration);
        _currentHp = phase.Hp;
    }
}
