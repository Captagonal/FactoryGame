using Godot;
using System;

public partial class Hub : StaticBody3D
{
	[Signal]
	public delegate void intakeSignalEventHandler(Item item);
	public override void _Ready()
	{

		
	}

	public override void _Process(double delta)
	{
		
	}

	internal void intake(Item item)
	{
		EmitSignalintakeSignal(item);
	}
}
