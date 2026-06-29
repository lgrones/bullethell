using Godot;

namespace bullethell.game.emitters.cadences;

[Tool]
[GlobalClass]
public partial class LinearCadence : Cadence
{
    [Export] public float ShotInterval = 0.3f;

    public override float NextInterval(int shotIndex, float elapsed)
        => ShotInterval;
}