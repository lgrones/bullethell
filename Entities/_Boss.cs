// using System.Collections.Generic;
// using bullethell.Emitters;
// using bullethell.Emitters.Patterns;
// using bullethell.Entities.Bullets;
// using Godot;
// using Timer = bullethell.Timers.Timer;
//
// namespace bullethell.Entities;
//
// public record Phase(float Duration, BulletEmitter[] Emitters);
//
// public class Boss
// {
//     public const float Radius = 8f;
//     public const float HitRadius = 10f;
//     public Vector2 Position;
//     public int Hp = 10;
//
//     private int _currentPhase;
//     private Timer? _phaseTimer;
//
//     private readonly Phase[] _phases =
//     [
//         new(Duration: 10f, [
//             new BulletEmitter(interval: 0.1f,
//                 new RepeatRotated(arms: 3,
//                     new Ring(arms: 4, speed: 120f,
//                         new BulletStyle(Radius: 6f, HitRadius: 6f, Colors.Aqua)))),
//             new BulletEmitter(interval: 0.3f,
//                 new Homing(speed: 200f,
//                     new BulletStyle(Radius: 10f, HitRadius: 6f, Colors.OrangeRed))),
//         ]),
//         new(Duration: 10f, [
//             new BulletEmitter(interval: 0.1f,
//                 new RepeatRotated(arms: 3,
//                     new Ring(arms: 10, speed: 150f,
//                         new BulletStyle(Radius: 6f, HitRadius: 6f, Colors.Aqua)))),
//         ])
//     ];
//
//
//     public void Spawn(Vector2 viewPort)
//         => Position = new Vector2(viewPort.X * 0.5f, viewPort.Y * 0.15f);
//
//     public void Update(List<Bullet> sink, in FrameContext ctx)
//     {
//         var phase = _phases[_currentPhase];
//         _phaseTimer ??= new Timer(phase.Duration);
//         
//         _phaseTimer.Update(in ctx);
//         
//         if (_phaseTimer.IsElapsed && _currentPhase < _phases.Length - 1)
//         {
//             phase = _phases[++_currentPhase];
//             _phaseTimer.Reset(phase.Duration);
//         }
//
//         foreach (var emitter in phase.Emitters)
//             emitter.Update(Position, sink, in ctx);
//     }
//     
//     public bool IsHit(Vector2 position, float hitRadius)
//     {
//         var r = HitRadius + hitRadius;
//         return Position.DistanceSquaredTo(position) < r * r;
//     }
// }