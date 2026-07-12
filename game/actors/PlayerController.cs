using Godot;

namespace bullethell.game.actors;

public partial class PlayerController : CharacterBody2D
{
    [Signal]
    public delegate void StaminaChangedEventHandler(float current, float max);

    private static readonly StringName MoveLeft = "ui_left";
    private static readonly StringName MoveRight = "ui_right";
    private static readonly StringName Jump = "jump";
    private static readonly StringName Dash = "dash";

    [Export] private float _speed = 280f;

    [Export] private float _dashSpeed = 500f;
    [Export] private float _dashDuration = 0.18f;
    [Export] private float _dashStaminaCost = 100f;

    [Export] private float _maxStamina = 200f;
    [Export] private float _staminaRecoverySpeed = 100f;

    [Export] private float _gravity = 1400f;
    [Export] private float _jumpVelocity = 520f;

    private float _dashTime;
    private float _currentStamina;
    private DashDirection _directionBuffer;

    protected bool IsDashing => _dashTime > 0f;

    public override void _Ready()
    {
        _currentStamina = _maxStamina;
        _directionBuffer = DashDirection.Right;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocityY = ProcessVerticalMovement((float)delta);
        var velocityX = ProcessHorizontalMovement((float)delta);

        Velocity = new Vector2(velocityX, velocityY);

        MoveAndSlide();
    }

    private float ProcessVerticalMovement(float delta)
    {
        if (!IsOnFloor())
            return Velocity.Y + _gravity * delta;

        if (Input.IsActionJustPressed(Jump))
            return -_jumpVelocity;

        return 0;
    }

    private float ProcessHorizontalMovement(float delta)
    {
        var direction = Input.GetAxis(MoveLeft, MoveRight);
        _directionBuffer = DashDirection.FromSignOrDefault(Mathf.Sign(direction), _directionBuffer);

        var staminaBefore = _currentStamina;

        if (_currentStamina < _maxStamina)
            _currentStamina = Mathf.Clamp(_currentStamina + _staminaRecoverySpeed * delta, 0, _maxStamina);

        if (Input.IsActionJustPressed(Dash) && _currentStamina >= _dashStaminaCost)
        {
            _dashTime = _dashDuration;
            _currentStamina -= _dashStaminaCost;
        }

        // One emit per frame, only when the value moved (recovery + dash can both
        // land on the same frame).
        if (!Mathf.IsEqualApprox(staminaBefore, _currentStamina))
            EmitSignal(SignalName.StaminaChanged, _currentStamina, _maxStamina);

        if (_dashTime <= 0f)
            return direction * _speed;

        _dashTime -= delta;
        return _directionBuffer * _dashSpeed;
    }

    private readonly struct DashDirection
    {
        public static readonly DashDirection Left = new(-1);
        public static readonly DashDirection Right = new(1);

        private readonly int _sign;
        private DashDirection(int sign) => _sign = sign;

        public static DashDirection FromSignOrDefault(float sign, DashDirection defaultValue)
            => sign == 0 ? defaultValue
                : sign < 0 ? Left
                : Right;

        public static implicit operator float(DashDirection direction) => direction._sign;
    }
}