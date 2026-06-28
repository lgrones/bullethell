using bullethell.Emitters;
using Godot;

namespace bullethell.Entities;

public partial class Phase : Node2D
{
	[Export] public float Duration;
	[Export] public uint Hp;
	[Export] public BulletEmitter[] Emitters = [];
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Disabled;
	}
}