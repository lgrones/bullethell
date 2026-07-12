using Godot;

namespace bullethell.main;

/// Title-screen overlay drawn over the first room. Play hands control back to
/// World (which unlocks the player); Options is a stub, Quit exits. No fade —
/// the tutorial room is already behind it, so starting is a continuous reveal.
public partial class MainMenu : CanvasLayer
{
    [Signal]
    public delegate void StartRequestedEventHandler();

    public override void _Ready()
    {
        GetNode<Button>("%Play").Pressed += () => EmitSignal(SignalName.StartRequested);
        GetNode<Button>("%Options").Pressed += OnOptions;
        GetNode<Button>("%Quit").Pressed += () => GetTree().Quit();
    }

    private static void OnOptions()
        => GD.Print("Options: not implemented yet"); // TODO: options panel
}
