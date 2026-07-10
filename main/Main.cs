using System.Linq;
using bullethell.game.actors;
using bullethell.game.actors.enemies;
using bullethell.game.bullets;
using bullethell.game.core;
using bullethell.game.emitters;
using Godot;

namespace bullethell.main;

/// Wires the two bullet fields, the boss, and the player together and owns the
/// run/lose/reset flow. Damage crosses via signals: enemy field -> player,
/// player field -> boss, player.Died / boss.Defeated -> here.
public partial class Main : Node2D
{
    [Export] public int Lives = 3;

    private BulletField _enemyField = null!;
    private BulletField _playerField = null!;
    private Boss _boss = null!;
    private Player _player = null!;
    private Hud _hud = null!;
    private readonly Lives _lives = new();

    public override void _Ready()
    {
        _enemyField = GetNode<BulletField>("EnemyField");
        _playerField = GetNode<BulletField>("PlayerField");
        _boss = (Boss)GetTree().GetNodesInGroup("bosses").First();
        _player = GetNode<Player>("Player");
        _hud = GetNode<Hud>("Hud");

        _lives.GameOver += () => GetTree().ReloadCurrentScene();

        // boss bullets damage the player
        _enemyField.Target = _player;
        _enemyField.Hit += _player.Hit;
        _player.Died += OnPlayerDied;

        // player shots damage the boss
        _playerField.Target = _boss;
        _playerField.Hit += _boss.Hit;

        // boss fires its phases into the enemy field and aims at the player
        _boss.Field = _enemyField;
        _boss.PlayerTarget = _player;
        _boss.Defeated += OnBossDefeated;

        foreach (var emitter in _player.FindEmitters())
        {
            emitter.Initialize(_playerField, _playerField.Table, _playerField.Styles);
            emitter.Disable();
            _player.Emitters.Add(emitter);
        }

        _boss.Begin();

        // Begin() builds the controller, so bind the HUD after it.
        _hud.Bind(_boss.Controller, _player, _lives);

        // Reset after Bind so the HUD catches the initial Changed emit that
        // builds the life icons.
        _lives.Reset(Lives);
    }

    // Player.Hit runs inside the field's collision iteration, so defer the pool
    // clear/respawn to avoid mutating the span mid-loop.
    private void OnPlayerDied() => CallDeferred(MethodName.LoseLife);

    private void LoseLife() => _lives.Lose();

    private void OnBossDefeated() => GetTree().ReloadCurrentScene();
}
