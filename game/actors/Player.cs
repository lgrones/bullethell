using System.Collections.Generic;
using bullethell.game.core;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.actors;

public partial class Player : PlayerController, ICollidable
{
    /// Raised when a bullet connects. Main clears the field and respawns/ends.
    [Signal]
    public delegate void DiedEventHandler();
    
    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 3f;
    [Export] public float InvincibleTime = 2f;

    private Vector2 _viewport;
    private float _iframes;
    public readonly List<BulletEmitter> Emitters = [];

    public override void _Ready()
    {
        base._Ready();
        
        _viewport = GetViewportRect().Size;
        GetViewport().SizeChanged += () => _viewport = GetViewportRect().Size;

        Respawn();
    }

    public override void _Process(double delta)
    {
        if (_iframes > 0f)
        {
            _iframes -= (float)delta;

            // Blink the sprite ~8x/sec while invincible; Modulate cascades to
            // the child sprite. Snap back to opaque the moment iframes end.
            var on = Mathf.PosMod(_iframes, 0.25f) < 0.125f;
            Modulate = new Color(1f, 1f, 1f, on ? 1f : 0.25f);
        }
        else
        {
            Modulate = Colors.White;
        }

        if (Input.IsActionJustPressed("fire"))
            Emitters.ForEach(x => x.Enable());
        else if (Input.IsActionJustReleased("fire"))
            Emitters.ForEach(x => x.Disable());
    }

    public void Respawn()
    {
        _iframes = 0f;
        Velocity = Vector2.Zero;
        Position = new Vector2(_viewport.X * 0.5f, _viewport.Y * 0.8f);
    }

    /// One bullet connected. Costs a life, then grants an invincibility window so
    /// a wall of bullets costs one life, not many. No reposition — the player
    /// keeps their spot; only a full reset repositions.
    public void Hit()
    {
        if (_iframes > 0f || IsDashing)
            return;

        _iframes = InvincibleTime;
        EmitSignal(SignalName.Died);
    }
}