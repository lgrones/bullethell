using System.Collections.Generic;
using bullethell.Emitters;
using Godot;

namespace bullethell.Entities;

public partial class Player : Node2D, ICollidable
{
    [Export] public float Speed = 280f;
    [Export] public float FocusSpeed = 110f;
    [Export] public float Radius = 8f;
    [Export] public float HitRadius { get; set; } = 3f;

    private Vector2 _viewport;
    public readonly List<BulletEmitter> Emitters = [];

    public override void _Ready()
    {
        _viewport = GetViewportRect().Size;
        GetViewport().SizeChanged += () => _viewport = GetViewportRect().Size;

        Respawn();
    }

    public override void _Process(double delta)
    {
        Move((float)delta);

        if (Input.IsActionJustPressed("fire"))
            Emitters.ForEach(x => x.Enable());
        else if (Input.IsActionJustReleased("fire"))
            Emitters.ForEach(x => x.Disable());

        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, Colors.White);

        if (IsFocused())
            DrawCircle(Vector2.Zero, HitRadius, Colors.Red);
    }

    public void Respawn()
        => Position = new Vector2(_viewport.X * 0.5f, _viewport.Y * 0.8f);

    public bool IsHit(Vector2 position, float hitRadius)
    {
        var r = HitRadius + hitRadius;
        return Position.DistanceSquaredTo(position) < r * r;
    }

    private static bool IsFocused()
        => Input.IsActionPressed("focus");

    private static bool IsFiring()
        => Input.IsActionPressed("fire");

    private void Move(float delta)
    {
        var direction = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down");

        var pos = Position + direction * delta * (IsFocused() ? FocusSpeed : Speed);
        pos.X = Mathf.Clamp(pos.X, Radius, _viewport.X - Radius);
        pos.Y = Mathf.Clamp(pos.Y, Radius, _viewport.Y - Radius);
        Position = pos;
    }
}