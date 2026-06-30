using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletState : Resource
{
    [Export]
    public MovementKind Move
    {
        get => _move;
        set
        {
            _move = value;
            NotifyPropertyListChanged();
        }
    }

    [Export] public float TurnRate = 3f;

    [Export]
    public TransitionKind Exit
    {
        get => _exit;
        set
        {
            _exit = value;
            NotifyPropertyListChanged();
        }
    }

    [Export] public float ExitThreshold;

    [Export]
    public DeathAction OnEnd
    {
        get => _onEnd;
        set
        {
            _onEnd = value;
            NotifyPropertyListChanged();
        }
    }

    [Export] public int ExplodeCount = 12;
    [Export] public float ExplodeSpeed = 150f;

    private MovementKind _move;
    private TransitionKind _exit;
    private DeathAction _onEnd;

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        var name = property["name"].AsStringName();

        if (name == PropertyName.TurnRate && _move != MovementKind.Homing)
            Hide(property);
        else if (name == PropertyName.ExitThreshold && _exit == TransitionKind.Never)
            Hide(property);
        else if ((name == PropertyName.ExplodeCount || name == PropertyName.ExplodeSpeed)
                 && _onEnd != DeathAction.Explode)
            Hide(property);
    }

    // Keep storing the value, just drop it from the inspector.
    private static void Hide(Godot.Collections.Dictionary property)
        => property["usage"] = (int)PropertyUsageFlags.Storage;
}