using Godot;

namespace bullethell.Entities;

public partial class Phase : Node2D
{
	[Export] public float Duration;
	[Export] public uint Hp;
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Disabled;
	}
}