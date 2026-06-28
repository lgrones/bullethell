using bullethell.Emitters;
using Godot;

namespace bullethell;

[Tool]
public partial class PreviewMain : Node2D
{
    public override void _Ready()
    {
        if (!Engine.IsEditorHint()) return;

        var field = GetNode<BulletField>("BulletField");
        var emitter = GetNode<BulletEmitter>("%Emitter");
        
        emitter.Sink = field.Bullets;
    }
}