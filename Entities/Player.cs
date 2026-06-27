using Godot;

namespace bullethell.Entities;

public class Player
{
    private const float Speed = 280f;
    private const float FocusSpeed = 110f;
    public const float Radius = 8f;
    public const float HitRadius = 3f;

    public Vector2 Position;

    public void Spawn(Vector2 viewPort)
        => Position = new Vector2(viewPort.X * 0.5f, viewPort.Y * 0.8f);

    public void Move(in FrameContext ctx)
    {
        var direction = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down");

        Position += direction * ctx.Delta * (IsFocused() ? FocusSpeed : Speed);
        Position.X = Mathf.Clamp(Position.X, 0, ctx.Viewport.X);
        Position.Y = Mathf.Clamp(Position.Y, 0, ctx.Viewport.Y);
    }

    public bool IsHit(Vector2 position, float hitRadius)
    {
        var r = HitRadius + hitRadius;
        return Position.DistanceSquaredTo(position) < r * r;
    }

    public static bool IsFocused()
        => Input.IsActionPressed("focus");
}