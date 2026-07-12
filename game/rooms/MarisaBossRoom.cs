using bullethell.game.actors.enemies;
using bullethell.game.emitters;
using Godot;

namespace bullethell.game.rooms;

/// Marisa's self-contained boss fight. Owns its boss node; wires it into the
/// shared bullet fields and starts phase 0 on Enter. The boss's Defeated becomes
/// the room's Cleared. One class per boss so each can add its own specifics.
public partial class MarisaBossRoom : Room
{
    private Boss _boss = null!;

    public override void _Ready()
        => _boss = GetNode<Boss>("Marisa");

    public override void Enter(in RoomContext ctx)
    {
        // seal the entrance behind the player — no walking back into the tutorial
        var seal = GetNodeOrNull<CollisionShape2D>("LeftSeal/CollisionShape2D");
        if (seal != null)
            seal.Disabled = false;

        // boss bullets damage the player; player shots damage the boss
        ctx.EnemyField.Target = ctx.Player;
        ctx.EnemyField.Hit += ctx.Player.Hit;
        ctx.PlayerField.Target = _boss;
        ctx.PlayerField.Hit += _boss.Hit;

        _boss.Field = ctx.EnemyField;
        _boss.PlayerTarget = ctx.Player;
        _boss.Defeated += () => EmitSignal(Room.SignalName.Cleared);

        // point the player's emitters at the shared player field
        foreach (var emitter in ctx.Player.FindEmitters())
        {
            emitter.Initialize(ctx.PlayerField, ctx.PlayerField.Table, ctx.PlayerField.Styles);
            emitter.Disable();
            if (!ctx.Player.Emitters.Contains(emitter))
                ctx.Player.Emitters.Add(emitter);
        }

        _boss.Begin();

        // Begin() builds the controller; bind HUD to it, then reset lives so the
        // HUD catches the initial Changed emit.
        ctx.Hud.Bind(_boss.Controller, ctx.Player, ctx.Lives);
        ctx.Lives.Reset(3); // TODO: World should own the life count + persist it across bosses
    }
}
