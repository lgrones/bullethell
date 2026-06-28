using System.Collections.Generic;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Resources;

[Tool]
public abstract partial class PatternResource : Resource
{
    public abstract void Emit(Vector2 origin, List<Bullet> sink, Vector2? target = null);
}