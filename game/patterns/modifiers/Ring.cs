using System.Collections.Generic;
using Godot;

namespace bullethell.game.patterns.modifiers;

[Tool]
[GlobalClass]
public partial class Ring : FrameModifier
{
    [Export] public int Count = 6;

    public override IEnumerable<Transform2D> Apply(IEnumerable<Transform2D> frames)
    {
        foreach (var f in frames)
            for (var i = 0; i < Count; i++)
                yield return f * new Transform2D(Mathf.Tau * i / Count, Vector2.Zero);
    }
}