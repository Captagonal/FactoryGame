using Godot;
using System;

public partial class BuildUi : Control
{
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void BuildEventHandler(MachineType type);
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void _on_conveyor_pressed(){
		EmitSignalBuild(MachineType.Conveyor);
	}
	public void _on_furnace_pressed(){
		EmitSignalBuild(MachineType.Furnace);

	}
	public void _on_miner_pressed(){
		EmitSignalBuild(MachineType.Miner);
	}
}
