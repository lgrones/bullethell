using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bullethell.game.bullets.behaviors;

public sealed class BehaviorTable
{
    private readonly List<StateDefinition> _defs = [];
    private readonly List<int> _offsets = [];
    private readonly List<int> _counts = [];
    private readonly Dictionary<BulletBehavior, byte> _ids = new();

    public ReadOnlySpan<StateDefinition> Defs => CollectionsMarshal.AsSpan(_defs);
    public int Offset(byte id) => _offsets[id];
    public int Count(byte id) => _counts[id];
    
    public BehaviorTable()
    {
        // id 0 = the implicit "no behavior" = single straight state, never transitions
        _offsets.Add(0);
        _counts.Add(1);
        _defs.Add(new StateDefinition {
            Move = MovementKind.Straight,
            Exit = TransitionKind.Never,
            OnEnd = DeathAction.Despawn,
        });
    }
    
    public byte IdFor(BulletBehavior? behavior)
    {
        if (behavior is null) return 0;

        if (_ids.TryGetValue(behavior, out var id)) return id;

        return Register(behavior);
    }

    private byte Register(BulletBehavior behavior)
    {
        var id = (byte)_offsets.Count;

        _ids[behavior] = id;
        _offsets.Add(_defs.Count);
        _counts.Add(behavior.States.Count);

        foreach (var state in behavior.States)
            _defs.Add(Bake(state));

        return id;
    }

    private static StateDefinition Bake(BulletState state)
        => new()
        {
            Move = state.Move,
            TurnRate = state.TurnRate,
            Exit = state.Exit,
            ExitThreshold = state.ExitThreshold,
            OnEnd = state.OnEnd,
            ExplodeCount = state.ExplodeCount,
            ExplodeSpeed = state.ExplodeSpeed,
        };
}