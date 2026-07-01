using Godot;

namespace bullethell.game.bullets;

public struct Bullet
{
    public Vector2 Position;
    public Vector2 Velocity;
    public byte BehaviorId;
    public byte StyleId;
    public byte StateIndex;
    public float StateTimer;
    public bool Alive;
}