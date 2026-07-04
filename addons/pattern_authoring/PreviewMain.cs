using bullethell.game.bullets;
using bullethell.game.emitters;
using Godot;

namespace bullethell.addons.pattern_authoring;

[Tool]
public partial class PreviewMain : Node2D
{
    public override void _Ready()
    {
        if (!Engine.IsEditorHint()) return;

        var field = GetNode<BulletField>("BulletField");

        foreach (var emitter in this.FindEmitters())
            emitter.Initialize(field, field.Table, field.Styles);
    }
}
