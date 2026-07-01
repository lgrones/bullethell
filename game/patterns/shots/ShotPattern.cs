using bullethell.game.bullets.behaviors;
using Godot;

namespace bullethell.game.patterns.shots;

[Tool]
[GlobalClass]
public abstract partial class ShotPattern : Pattern
{
    [Export] public BulletBehavior? Behavior;
    [Export] public BulletStyleResource? Style;
    [Export] public float Speed = 200f;
}
