using bullethell.game.actors;
using bullethell.game.bullets;
using bullethell.game.core;
using bullethell.main;
using Godot;

namespace bullethell.game.rooms;

/// What a room needs from the persistent World to run: the shared player, HUD,
/// lives, and the two bullet fields. The fields stay at World origin so bullets
/// render in world space, so they're a shared service rooms fire into rather
/// than something each room owns.
public readonly record struct RoomContext(
    Player Player,
    Hud Hud,
    Lives Lives,
    BulletField EnemyField,
    BulletField PlayerField,
    int LifeCount);

/// One streamed room in the continuous world. World lays these out side by side;
/// the player/camera/HUD/fields persist across them. Boss rooms start their
/// fight in Enter and raise Cleared when won.
public partial class Room : Node2D
{
    [Signal]
    public delegate void ClearedEventHandler();

    /// Width in world units; World uses it to place the next room's left edge.
    [Export] public float Width = 640f;

    /// Called by World once the player crosses into this room. Base (e.g.
    /// tutorial) does nothing; boss rooms override to wire and start the fight.
    public virtual void Enter(in RoomContext ctx) { }
}
