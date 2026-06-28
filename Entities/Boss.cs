using System.Collections.Generic;
using bullethell.Entities.Bullets;
using Godot;
using Timer = bullethell.Timers.Timer;

namespace bullethell.Entities;

public partial class Boss : Node2D, ICollidable
{
    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 10f;
    [Export] public Phase[] Phases = [];

    public List<Bullet> Field = [];

    private int _currentPhase;
    private uint _currentHp;
    private Timer _phaseTimer = null!;

    public override void _Ready()
    {
        var viewPort = GetViewportRect().Size;
        Position = new Vector2(viewPort.X * 0.5f, viewPort.Y * 0.15f);

        EnablePhase();
    }

    public override void _Process(double delta)
    {
        if (_currentPhase == Phases.Length)
            return;
        
        _phaseTimer.Update((float)delta);

        if (_currentHp == 0 || _phaseTimer.IsElapsed)
            NextPhase();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, Colors.GreenYellow);
    }

    public bool IsHitBy(Vector2 position, float hitRadius)
    {
        var isHit = ICollidable.Overlaps(Position, hitRadius, position, hitRadius);

        if (isHit && _currentHp > 0)
            _currentHp--;

        return isHit;
    }

    private void NextPhase()
    {
        Phases[_currentPhase].ProcessMode = ProcessModeEnum.Disabled;
        Field.Clear();

        _currentPhase++;

        if (_currentPhase == Phases.Length)
            return;

        EnablePhase();
    }

    private void EnablePhase()
    {
        _phaseTimer = new Timer(Phases[_currentPhase].Duration);
        _currentHp = Phases[_currentPhase].Hp;
        Phases[_currentPhase].ProcessMode = ProcessModeEnum.Inherit;
    }
}