using bullethell.game.emitters;
using Godot;

namespace bullethell.game.patterns;

[Tool]
[GlobalClass]
public abstract partial class Pattern : Resource
{
     public abstract void Emit(EmitContext ctx, in Transform2D frame);
}