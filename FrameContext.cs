using Godot;

namespace bullethell;

public struct FrameContext
{
    public required float Delta { get; init; }                                                                                                                                                                                                                                                                 
    public required Vector2 Viewport { get; init; }                                                                                                                                                                                                                                                            
    public required Vector2 PlayerPosition { get; init; }
}