using Godot;

namespace bullethell.game.rooms;

/// The opening room: teaches the controls with a few floating hint labels, then
/// the player walks right into the first boss. No boss here, so Enter stays the
/// base no-op. Positions are room-local; tweak the table to taste.
public partial class TutorialRoom : Room
{
    private static readonly (string Text, Vector2 Pos)[] Hints =
    {
        ("← →\nMove", new Vector2(60, 170)),
        ("Space\nJump", new Vector2(200, 170)),
        ("Shift\nDash", new Vector2(330, 170)),
        ("Z\nFire", new Vector2(450, 170)),
        ("Boss →", new Vector2(560, 150)),
    };

    public override void _Ready()
    {
        foreach (var (text, pos) in Hints)
        {
            var label = new Label
            {
                Text = text,
                Position = pos,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            // outline so hints read over any floor/background
            label.AddThemeConstantOverride("outline_size", 4);
            label.AddThemeColorOverride("font_outline_color", Colors.Black);
            AddChild(label);
        }
    }
}
