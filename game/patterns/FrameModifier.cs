using System.Collections.Generic;
using Godot;

namespace bullethell.game.patterns;

[Tool]
[GlobalClass]
public abstract partial class FrameModifier : Resource
{
    public abstract IEnumerable<Transform2D> Apply(IEnumerable<Transform2D> frames);
}