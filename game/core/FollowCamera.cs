using Godot;

namespace bullethell.game.core;

/// Tracks the player horizontally within a room. The camera is a World sibling of
/// the player (not its child), so it can't use Camera2D's parent-follow — it reads
/// the target's X each frame. Vertical stays locked to the authored Y. Bounds and
/// smoothing are the engine's (LimitLeft/Right + PositionSmoothing).
public partial class FollowCamera : Camera2D
{
    [Export] private Node2D? _target;
    [Export] private float _smooth = 6f;

    public override void _Ready()
    {
        PositionSmoothingEnabled = true;
        PositionSmoothingSpeed = _smooth;
        MakeCurrent();
    }

    public override void _Process(double delta)
    {
        if (_target == null)
            return;

        // Feed the raw target X; the engine smooths the view and clamps it to the
        // room limits. Y is held at the authored value so jumping doesn't scroll.
        GlobalPosition = new Vector2(_target.GlobalPosition.X, GlobalPosition.Y);
    }

    /// Re-clamp the view to a room, e.g. locking to a boss arena on entry. Widening
    /// the range lets the built-in smoothing pan across with no cut. A room narrower
    /// than the view pins the camera to the room's center.
    public void SetLimits(float left, float right)
    {
        var viewWidth = GetViewportRect().Size.X / Zoom.X;

        if (right - left < viewWidth)
        {
            var center = (left + right) * 0.5f;
            left = center - viewWidth * 0.5f;
            right = center + viewWidth * 0.5f;
        }

        LimitLeft = Mathf.RoundToInt(left);
        LimitRight = Mathf.RoundToInt(right);
    }
}
