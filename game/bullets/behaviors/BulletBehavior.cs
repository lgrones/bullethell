using Godot;

namespace bullethell.game.bullets.behaviors;

[Tool]
[GlobalClass]
public partial class BulletBehavior : Resource
{
    [Export] public Godot.Collections.Array<BulletState> States = [];
}