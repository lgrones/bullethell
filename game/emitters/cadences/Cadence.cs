using Godot;

namespace bullethell.game.emitters.cadences;

[Tool]
[GlobalClass]
public abstract partial class Cadence : Resource
{
    public abstract float NextInterval(int shotIndex, float elapsed);
}