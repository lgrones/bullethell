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

        // The fields are World-owned and outlive this room, so hold locals to
        // undo the wiring on defeat — otherwise the next boss room's fields would
        // still carry this fight's hit handlers.
        var enemyField = ctx.EnemyField;
        var playerField = ctx.PlayerField;
        var player = ctx.Player;

        // boss bullets damage the player; player shots damage the boss
        enemyField.Target = player;
        enemyField.Hit += player.Hit;
        playerField.Target = _boss;
        playerField.Hit += _boss.Hit;

        _boss.Defeated += OnDefeated;

        // point the player's emitters at the shared player field
        foreach (var emitter in player.FindEmitters())
        {
            emitter.Initialize(playerField, playerField.Table, playerField.Styles);
            emitter.Disable();
            if (!player.Emitters.Contains(emitter))
                player.Emitters.Add(emitter);
        }

        _boss.Begin(enemyField, player);

        // Begin() builds the controller (non-null from here); bind HUD to it, then
        // reset lives so the HUD catches the initial Changed emit.
        ctx.Hud.Bind(_boss.Controller!, player, ctx.Lives);
        ctx.Lives.Reset(ctx.LifeCount);
        return;

        void OnDefeated()
        {
            enemyField.Hit -= player.Hit;
            playerField.Hit -= _boss.Hit;
            EmitSignal(Room.SignalName.Cleared);
        }
    }
}
