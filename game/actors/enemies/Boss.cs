using bullethell.game.bullets;
using bullethell.game.core;
using bullethell.game.emitters;
using bullethell.game.phases;
using Godot;

namespace bullethell.game.actors.enemies;

public partial class Boss : Node2D, ICollidable
{
    /// Raised when the last phase ends. Main turns this into a win/reset.
    [Signal]
    public delegate void DefeatedEventHandler();

    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 10f;
    [Export] public float HpRingRadius = 60f;
    [Export] public PackedScene[] Phases = [];

    /// Field the phase emitters fire into, and the player they aim at. Set by Main.
    public BulletField Field = null!;
    public Node2D? PlayerTarget;

    /// The fight's signal hub — Main hands this to the HUD. Built in Begin().
    public PhaseController? Controller { get; private set; }

    private BossPhase? _phase;

    /// Kicks off phase 0. Called by Main after Field is wired (Boss._Ready runs
    /// before Main._Ready, so this can't live in _Ready).
    public void Begin()
    {
        Controller = new PhaseController(Phases.Length);
        Controller.PhaseEntered += OnPhaseEntered;
        Controller.Defeated += () => EmitSignal(SignalName.Defeated);
        Controller.Health.Changed += (_, _) => QueueRedraw();
        Controller.Start();
    }

    public override void _Process(double delta)
    {
        if (Controller == null)                                                                                 
            return;  
        
        Controller.Update((float)delta);
    }

    public override void _Draw()
    {
        if (Controller == null)                                                                                 
            return;
        
        DrawArc(Vector2.Zero, HpRingRadius, 0f, Mathf.Tau, 48,
            new Color(0f, 0f, 0f, 0.35f), 5f, true);

        var fraction = Controller.Health.Fraction;
        var end = -Mathf.Pi / 2f + Mathf.Tau * fraction;
        DrawArc(Vector2.Zero, HpRingRadius, -Mathf.Pi / 2f, end, 48,
            Colors.Crimson, 5f, true);
    }

    /// One bullet connected. Drains phase HP; the controller advances when it empties.
    public void Hit() => Controller?.Damage();

    /// Controller entered a phase: swap in its scene, wire emitters, feed its HP/duration.
    private void OnPhaseEntered(int index)
    {
        if (_phase is not null)
        {
            _phase.QueueFree();
            Field.Clear();
        }

        var phase = Phases[index].Instantiate<BossPhase>();
        AddChild(phase);

        foreach (var emitter in phase.FindEmitters())
        {
            emitter.Target = PlayerTarget;
            emitter.Initialize(Field, Field.Table, Field.Styles);
        }

        _phase = phase;
        Controller?.Configure((int)phase.Hp, phase.Duration);
    }
}
