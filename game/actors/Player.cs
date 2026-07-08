using System.Collections.Generic;
using bullethell.game.core;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.actors;

public partial class Player : CharacterBody2D, ICollidable
{
    /// Raised when a bullet connects. Main clears the field and respawns/ends.
    [Signal]
    public delegate void DiedEventHandler();

    [Export] public float Speed = 280f;
    [Export] public float FocusSpeed = 110f;
    [Export] public float Gravity = 1400f;
    [Export] public float JumpVelocity = 520f;
    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 3f;
    [Export] public float InvincibleTime = 2f;

    private Vector2 _viewport;
    private float _iframes;
    public readonly List<BulletEmitter> Emitters = [];

    public override void _Ready()
    {
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

        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // Gravity accelerates us downward while airborne (+Y is down).                                                                                                                                                                                                                                        
        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;

        // Jump only off the ground, only on the press edge (-Y is up).                                                                                                                                                                                                                                        
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            velocity.Y = -JumpVelocity;

        // Horizontal walk. GetAxis returns -1..1 from the two actions.                                                                                                                                                                                                                                        
        var move = Input.GetAxis("ui_left", "ui_right");
        velocity.X = move * (IsFocused() ? FocusSpeed : Speed);

        Velocity = velocity;
        MoveAndSlide();

        // No side walls yet — keep the player on-screen horizontally.                                                                                                                                                                                                                                         
        var p = Position;
        p.X = Mathf.Clamp(p.X, Radius, _viewport.X - Radius);
        Position = p;
    }

    public override void _Draw()
    {
        // Body is the Sprite2D child now; only the focus hitbox dot is drawn,
        // on top of the sprite (which sits behind via show_behind_parent).
        if (IsFocused())
            DrawCircle(Vector2.Zero, HitRadius, Colors.Red);
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
        if (_iframes > 0f)
            return;

        _iframes = InvincibleTime;
        EmitSignal(SignalName.Died);
    }

    private static bool IsFocused()
        => Input.IsActionPressed("focus");
}