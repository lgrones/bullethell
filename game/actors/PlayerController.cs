using Godot;

namespace bullethell.game.actors;

public partial class PlayerController : CharacterBody2D
{
    [Export] private float _speed = 280f;
    [Export] private float _dashSpeed = 500f;
    [Export] private float _dashDuration = 0.18f;
    [Export] private float _gravity = 1400f;
    [Export] private float _jumpVelocity = 520f;

    private float _dashTime;
    private float _dashDir;

    protected bool IsDashing => _dashTime > 0f;

    public override void _PhysicsProcess(double delta)
    {
        var deltaY = ProcessVerticalMovement((float)delta);
        var deltaX = ProcessHorizontalMovement((float)delta);

        Velocity = new Vector2(deltaX, deltaY);

        MoveAndSlide();
    }

    private float ProcessVerticalMovement(float delta)
    {
        if (!IsOnFloor())
            return Velocity.Y + _gravity * delta;

        if (Input.IsActionJustPressed("jump"))
            return -_jumpVelocity;

        return 0;
    }

    private float ProcessHorizontalMovement(float delta)
    {
        var direction = Input.GetAxis("ui_left", "ui_right");

        if (Input.IsActionJustPressed("dash"))
        {
            _dashTime = _dashDuration;
            _dashDir = direction != 0f
                ? Mathf.Sign(direction)
                : _dashDir != 0f
                    ? _dashDir                                                                                                                                                                                                         
                    : 1f;                                                                                                                                                                                                                  
        }

        if (_dashTime > 0f)
        {
            _dashTime -= delta;
            return _dashDir * _dashSpeed;
        }


        if (direction != 0f) _dashDir = Mathf.Sign(direction);

        return direction * _speed;
    }
}