using bullethell.game.bullets.behaviors;
using bullethell.game.bullets.systems;
using bullethell.game.core;
using Godot;

namespace bullethell.game.bullets;

/// Owns the bullet pool and runs the per-frame system pipeline.
/// Emitters push spawns via IBulletSink; systems do the work.
[Tool]
public sealed partial class BulletField : Node2D, IBulletSink
{
    /// Fires once per bullet that hits Target. Wire to the target's damage handler.
    [Signal]
    public delegate void HitEventHandler();

    [Export] public int MaxBullets = 4096;

    /// Homing/aimed target for the simulation, and the collision victim if it's
    /// an ICollidable (Player or Boss). One node serves both roles.
    [Export] public Node2D? Target;

    public BehaviorTable Table = new();
    public StyleTable Styles = new();

    private BulletPool _pool = null!;
    private SimulationSystem _simulation = null!;
    private CollisionSystem _collision = null!;
    private CullSystem _cull = null!;
    private RenderSystem _render = null!;
    private Rect2 _bounds;
    private float _time;

    public override void _Ready()
    {
        _pool = new BulletPool(MaxBullets);
        _simulation = new SimulationSystem(Table);
        _collision = new CollisionSystem(Styles) { OnHit = () => EmitSignal(SignalName.Hit) };
        _cull = new CullSystem(Styles);
        _render = new RenderSystem(this, Styles, MaxBullets);

        var viewport = GetViewportRect().Size;
        _bounds = new Rect2(-64, -64, viewport.X + 128, viewport.Y + 128);
    }

    public override void _PhysicsProcess(double delta)
    {
        _time += (float)delta;

        var frame = new BulletFrame(
            (float)delta,
            _time,
            Target?.GlobalPosition ?? Vector2.Zero,
            Target is not null,
            _bounds);

        _collision.Target = Target as ICollidable;

        _pool.Flush();                  // admit emitter spawns
        _simulation.Run(_pool, frame);  // move + state machine + explosions
        _pool.Flush();                  // admit explosion spawns
        _collision.Run(_pool, frame);   // hit Target -> dead + Hit signal
        _cull.Run(_pool, frame);        // bounds -> dead
        _pool.Compact();                // drop dead
        _render.Run(_pool, frame);      // draw survivors
    }

    public override void _Draw()
    {
        if (Engine.IsEditorHint())
            DrawRect(_bounds, Colors.Red, filled: false);
    }

    public void SpawnBullet(in BulletSpawn bullet) => _pool.SpawnBullet(bullet);

    public void Clear() => _pool.Clear();
}
