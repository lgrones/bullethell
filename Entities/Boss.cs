using Godot;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050
public partial class Boss : Node2D
#pragma warning restore CA1050
{
    [Export] public float Radius = 8f;
    [Export] public float HitRadius = 10f;

    public int Hp = 10;

    public override void _Ready()
    {
        var viewPort = GetViewportRect().Size;
        Position = new Vector2(viewPort.X * 0.5f, viewPort.Y * 0.15f);
    }
    
    public bool IsHit(Vector2 position, float hitRadius)
    {
        var r = HitRadius + hitRadius;
        return Position.DistanceSquaredTo(position) < r * r;
    }
}