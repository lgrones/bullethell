using Godot;

namespace bullethell.game.emitters.cadences;

[Tool]
[GlobalClass]
public partial class BurstCadence : Cadence
{
    [Export] public int ShotsPerBurst = 5;
    [Export] public float ShotInterval = 0.04f;
    [Export] public float RestInterval = 0.8f;

    public override float NextInterval(int shotIndex, float elapsed)
        => (shotIndex + 1) % ShotsPerBurst == 0 ? RestInterval : ShotInterval;
}