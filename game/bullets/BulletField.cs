using bullethell.game.bullets.behaviors;
using bullethell.game.bullets.systems;
using Godot;

namespace bullethell.game.bullets;

/// Owns the bullet pool and runs the per-frame system pipeline.
/// Emitters push spawns via IBulletSink; systems do the work.
[Tool]
public sealed partial class BulletField : Node2D, IBulletSink
{
    [Export] public int MaxBullets = 4096;
    [Export] public Node2D? Player;
    [Export] public Texture2D? Atlas;
    [Export] private MultiMeshInstance2D _multiMeshInstance = null!;

    public BehaviorTable Table = new();
    public StyleTable Styles = new();

    private BulletPool _pool = null!;
    private SimulationSystem _simulation = null!;
    private CullSystem _cull = null!;
    private RenderSystem _render = null!;
    private Rect2 _bounds;
    private float _time;

    public override void _Ready()
    {
        _pool = new BulletPool(MaxBullets);
        _simulation = new SimulationSystem(Table);
        _cull = new CullSystem();
        _render = new RenderSystem(_multiMeshInstance, Styles, Atlas, MaxBullets);

        var viewport = GetViewportRect().Size;
        _bounds = new Rect2(-64, -64, viewport.X + 128, viewport.Y + 128);
    }

    public override void _PhysicsProcess(double delta)
    {
        _time += (float)delta;

        var frame = new BulletFrame(
            (float)delta,
            _time,
            Player?.GlobalPosition ?? Vector2.Zero,
            Player is not null,
            _bounds);

        _pool.Flush();                  // admit emitter spawns
        _simulation.Run(_pool, frame);  // move + state machine + explosions
        _pool.Flush();                  // admit explosion spawns
        _cull.Run(_pool, frame);        // bounds -> dead
        // future: _collision.Run(_pool, frame);
        _pool.Compact();                // drop dead
        _render.Run(_pool, frame);      // draw survivors
    }

    public override void _Draw()
    {
        if (Engine.IsEditorHint())
            DrawRect(_bounds, Colors.Red, filled: false);
    }

    public void SpawnBullet(in BulletSpawn bullet) => _pool.SpawnBullet(bullet);
}
