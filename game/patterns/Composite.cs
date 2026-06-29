using System;
using System.Collections.Generic;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.patterns;

[Tool]
[GlobalClass]
public partial class Composite : Pattern
{
    [Export] public Pattern? Source;
    [Export] public Godot.Collections.Array<FrameModifier> Modifiers = [];

    public override void Emit(EmitContext ctx, in Transform2D frame)
    {
        if (Source is null)
            throw new InvalidOperationException($"Composite not wired. Assign a {nameof(Source)} pattern");

        IEnumerable<Transform2D> frames = [Transform2D.Identity];

        foreach (var modifier in Modifiers)
            frames = modifier.Apply(frames);

        foreach (var local in frames)
            Source.Emit(ctx, frame * local);
    }
}