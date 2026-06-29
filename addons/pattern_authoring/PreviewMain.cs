using bullethell.game.emitters;
using Godot;

namespace bullethell.addons.pattern_authoring;

[Tool]
public partial class PreviewMain : Node2D
{
    public override void _Ready()
    {
        if (!Engine.IsEditorHint()) return;

        var field = GetNode<game.bullets.BulletField>("BulletField");
        var emitter = GetNode<BulletEmitter>("%Emitter");
        
        emitter.Initialize(field, field.Table);
    }
}