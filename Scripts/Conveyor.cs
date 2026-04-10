using Godot;
using System;
using System.IO;

public partial class Conveyor : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	private Path3D path;
	public override void _Ready()
	{
		path = GetNode<Path3D>("Path3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		for (int i = 0; i < path.GetChildCount(); i++)
		{
			Node child = path.GetChild(i);
			if (child is PathFollow3D pathFollow3D)
			{
				pathFollow3D.ProgressRatio += (float)delta * 1;
				if (pathFollow3D.ProgressRatio >= .98)
				{
					pathFollow3D.ProgressRatio = 0;
					pathFollow3D.QueueFree();
				}
			}
		}
	}

	public void takeInItem(Item item)
	{
		PathFollow3D pathFollow3D = new PathFollow3D();
		path.AddChild(pathFollow3D);
		item.Reparent(pathFollow3D);
	}
}
