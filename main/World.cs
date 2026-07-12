using bullethell.game.actors;
using bullethell.game.bullets;
using bullethell.game.core;
using bullethell.game.rooms;
using Godot;

namespace bullethell.main;

/// Persistent root of the continuous world. Holds the things that survive across
/// rooms — player, camera, HUD, lives, and the two shared bullet fields — and
/// drives the flow. Rooms are the ordered Room children under "Rooms", laid out
/// left to right by their Position.X. Crossing into a room Enters it (boss rooms
/// start their fight); no scene swaps, so each transition is a continuous pan.
public partial class World : Node2D
{
    [Export] public int Lives = 3;

    /// How far past a room's left edge the player must be before it activates.
    [Export] public float EntryMargin = 40f;

    private Player _player = null!;
    private FollowCamera _camera = null!;
    private Hud _hud = null!;
    private BulletField _enemyField = null!;
    private BulletField _playerField = null!;
    private Node2D _roomHost = null!;
    private MainMenu _menu = null!;
    private readonly Lives _lives = new();

    private readonly System.Collections.Generic.List<Room> _rooms = [];
    private int _current = -1;

    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _camera = GetNode<FollowCamera>("Camera");
        _hud = GetNode<Hud>("Hud");
        _enemyField = GetNode<BulletField>("EnemyField");
        _playerField = GetNode<BulletField>("PlayerField");
        _roomHost = GetNode<Node2D>("Rooms");
        _menu = GetNode<MainMenu>("Menu");

        _lives.GameOver += () => GetTree().ReloadCurrentScene();
        _player.Died += OnPlayerDied;
        _menu.StartRequested += StartGame;

        foreach (var child in _roomHost.GetChildren())
            if (child is Room room)
                _rooms.Add(room);

        EnterRoom(0);

        // hold the player on the title screen until Play
        _player.ProcessMode = ProcessModeEnum.Disabled;
    }

    private void StartGame()
    {
        _menu.Visible = false;   // hidden overlay stops capturing input; room already behind it
        _player.ProcessMode = ProcessModeEnum.Inherit;
    }

    public override void _Process(double delta)
    {
        var next = _current + 1;
        if (next >= _rooms.Count)
            return;

        // activate the next room once the player is a little past its left edge
        if (_player.GlobalPosition.X < _rooms[next].Position.X + EntryMargin)
            return;

        EnterRoom(next);
    }

    private void EnterRoom(int i)
    {
        _current = i;
        var room = _rooms[i];
        var left = room.Position.X;

        _camera.SetLimits(left, left + room.Width);   // lock/pan the view to this room
        room.Cleared += () => OnRoomCleared(i);
        room.Enter(new RoomContext(_player, _hud, _lives, _enemyField, _playerField));
    }

    // Player.Hit fires inside the field's collision iteration; defer the life
    // loss so the pool clear/respawn doesn't mutate the span mid-loop.
    private void OnPlayerDied() => CallDeferred(MethodName.LoseLife);

    private void LoseLife() => _lives.Lose();

    // Last room cleared -> reset for now. Later: open the exit to room i+1.
    private void OnRoomCleared(int i) => GetTree().ReloadCurrentScene();
}
