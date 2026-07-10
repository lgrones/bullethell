using System.Collections.Generic;
using bullethell.game.actors;
using bullethell.game.actors.enemies;
using bullethell.game.core;
using Godot;

namespace bullethell.main;

/// Screen-space HUD. Owns no state — binds once to the fight's PhaseController
/// and the run's Lives, then reacts to their signals. The boss ring healthbar
/// lives on the Boss node (world space), not here.
public partial class Hud : CanvasLayer
{
    [Export] private Texture2D? _lifeIcon;

    private const int IconSize = 24;

    private ProgressBar _phaseBar = null!;
    private ProgressBar _staminaBar = null!;
    private HBoxContainer _lifeRow = null!;
    private readonly List<TextureRect> _icons = [];

    public override void _Ready()
    {
        _phaseBar = GetNode<ProgressBar>("PhaseBar");
        _staminaBar = GetNode<ProgressBar>("StaminaBar");
        _lifeRow = GetNode<HBoxContainer>("LifeRow");
    }

    public void Bind(PhaseController phaseController, PlayerController playerController, Lives lives)
    {
        phaseController.Timer.Ticked += OnTick;
        lives.Changed += OnLivesChanged;
        playerController.StaminaChanged += OnStaminaChanged;
    }

    private void OnTick(float remaining, float duration)
    {
        _phaseBar.MaxValue = duration;
        _phaseBar.Value = remaining;
    }

    private void OnStaminaChanged(float current, float max)
    {
        _staminaBar.MaxValue = max;
        _staminaBar.Value = current;
    }

    private void OnLivesChanged(int current, int max)
    {
        while (_icons.Count < max)
        {
            var icon = new TextureRect
            {
                Texture = _lifeIcon,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                CustomMinimumSize = new Vector2(IconSize, IconSize),
            };
            
            _lifeRow.AddChild(icon);
            _icons.Add(icon);
        }

        for (var i = 0; i < _icons.Count; i++)
            _icons[i].Visible = i < current;
    }
}
