using System;
using System.Collections.Generic;
using bullethell.Emitters;
using bullethell.Emitters.Patterns;
using Godot;

namespace bullethell.Entities;

public record Phase(float Duration, Emitter[] Emitters);

public class Boss
{
    public const float Radius = 8f;
    public Vector2 Position;

    private int _currentPhase;
    private float _phaseTimer;

    private readonly Phase[] _phases =
    [
        new(Duration: 10f, [
            new Emitter(interval: 0.1f,
                new RepeatRotated(arms: 3,
                    new Spiral(startAngle: 0f, turn: 0.3f, speed: 120f,
                        new BulletStyle(Radius: 6f, HitRadius: 6f, Colors.Aqua)))),
            new Emitter(interval: 0.3f,
                new Homing(speed: 200f,
                    new BulletStyle(Radius: 10f, HitRadius: 6f, Colors.OrangeRed))),
        ]),
        new(Duration: 10f, [
            new Emitter(interval: 0.1f,
                new RepeatRotated(arms: 3,
                    new Ring(arms: 10, speed: 150f,
                        new BulletStyle(Radius: 6f, HitRadius: 6f, Colors.Aqua)))),
            new Emitter(interval: 0.3f,
                new AimedAt(
                    new Spiral(startAngle: 0f, turn: 0.3f, speed: 120f,
                        new BulletStyle(Radius: 10f, HitRadius: 6f, Colors.OrangeRed))
                )),
        ])
    ];


    public void Spawn(Vector2 viewPort)
        => Position = new Vector2(viewPort.X * 0.5f, viewPort.Y * 0.15f);

    public void Fire(List<Bullet> sink, in FrameContext ctx)
    {
        _phaseTimer += ctx.Delta;

        var phase = _phases[_currentPhase];

        if (_phaseTimer >= phase.Duration && _currentPhase < _phases.Length - 1)
        {
            _phaseTimer -= phase.Duration;
            phase = _phases[++_currentPhase];
        }

        foreach (var emitter in phase.Emitters)
            emitter.Update(Position, sink, in ctx);
    }
}