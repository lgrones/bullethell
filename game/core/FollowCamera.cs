using Godot;

namespace bullethell.game.core;

public partial class FollowCamera : Camera2D
{
    [Export] private float _rightLimit = 640f;
    [Export] private Node2D? _target;
    [Export] private float _leftLimit;
    [Export] private float _smooth = 6f;

    public override void _Ready()
        => MakeCurrent();

    public override void _Process(double delta)
    {
        if (_target == null)
            return;

        var half = GetViewportRect().Size.X * 0.5f / Zoom.X;
        var min = _leftLimit + half;
        var max = _rightLimit - half;

        var targetX = max < min
            ? (_leftLimit + _rightLimit) * 0.5f
            : Mathf.Clamp(_target.GlobalPosition.X, min, max);

        var weight = 1f - Mathf.Exp(-_smooth * (float)delta);
        GlobalPosition = new Vector2(Mathf.Lerp(GlobalPosition.X, targetX, weight), GlobalPosition.Y);
    }
    
    /// Re-clamp the view, e.g. locking to a boss arena on entry. Widening the
    /// range lets the camera pan across to the new bounds with no cut.
    public void SetLimits(float left, float right)
    {
        _leftLimit = left;
        _rightLimit = right;
    }
}