using System.Collections.Generic;
using bullethell.Entities.Bullets;
using Godot;

namespace bullethell.Emitters.Patterns;

public interface IEmitPattern
{
    void Emit(Vector2 origin, List<Bullet> sink, in FrameContext ctx);
}