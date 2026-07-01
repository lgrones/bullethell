using System.Collections.Generic;
using Godot;

namespace bullethell.game.emitters;

public static class EmitterExtensions
{
    /// Every BulletEmitter in the subtree, depth-first. Use to initialize a
    /// whole phase/scene at once instead of wiring emitters by path.
    public static IEnumerable<BulletEmitter> FindEmitters(this Node root)
    {
        foreach (var child in root.GetChildren())
        {
            if (child is BulletEmitter emitter)
                yield return emitter;

            foreach (var nested in child.FindEmitters())
                yield return nested;
        }
    }
}
