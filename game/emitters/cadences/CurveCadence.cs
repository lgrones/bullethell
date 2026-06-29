using Godot;

namespace bullethell.game.emitters.cadences;

public partial class CurveCadence : Cadence
{
    [Export] public Curve RatePerSecond = new();     
    [Export] public float Duration = 10f;

    public override float NextInterval(int shotIndex, float elapsed)
    {
        var rate = Mathf.Max(0.5f, RatePerSecond.Sample(elapsed / Duration));
        return 1f / rate;
    }
}