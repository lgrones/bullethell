using Godot;

namespace bullethell.game.emitters;

/// Spins its children around this node's origin. Parent emitters here, offset
/// by their local position (= orbit radius), and they orbit the anchor — e.g.
/// rotating rings around the boss. Emission reads GlobalTransform, so the
/// motion propagates for free; no per-emitter code needed.
[Tool]
public sealed partial class EmitterAnchor : Node2D
{
    /// Degrees per second. Positive = counter-clockwise. 0 = static offset.
    [Export] public float RotationSpeed;

    public override void _PhysicsProcess(double delta)
        => RotationDegrees = Mathf.PosMod(RotationDegrees + RotationSpeed * (float)delta, 360f);
}
