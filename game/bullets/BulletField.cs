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

        if (Engine.IsEditorHint())
            _bounds = new Rect2(-500, -500, 1000, 1000);
    }

    public override void _PhysicsProcess(double delta)
    {
        _time += (float)delta;

        // Bullets live in world coords; cull against what the camera sees so the
        // bounds follow the active room no matter where it sits in the world.
        if (!Engine.IsEditorHint())
            _bounds = VisibleWorldBounds();

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

    /// The camera's visible world rect, grown so a bullet lives until it's fully
    /// off-screen. Following the view keeps culling correct wherever the room is.
    private Rect2 VisibleWorldBounds()
    {
        var toWorld = GetViewport().CanvasTransform.AffineInverse();
        var size = GetViewportRect().Size;
        var topLeft = toWorld * Vector2.Zero;
        var bottomRight = toWorld * size;
        return new Rect2(topLeft, bottomRight - topLeft).Grow(64f);
    }

    public void SpawnBullet(in BulletSpawn bullet) => _pool.SpawnBullet(bullet);

    public void Clear() => _pool.Clear();
}
