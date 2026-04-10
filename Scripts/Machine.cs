using Godot;
using System;

public partial class Machine : Conveyor
{
	// Called when the node enters the scene tree for the first time.


	public override void _Process(double delta)
	{
		base._Process(delta);
		// Additional processing logic for the machine can be added here
		if (path == null || path.GetChildCount() == 0) return;

		for (int i = 0; i < path.GetChildCount(); i++)
		{
			Node child = path.GetChild(i);
			if (child is PathFollow3D pathFollow3D)
			{
				// Check if the item is within the machine's processing area (e.g., near the center of the path)
				if (pathFollow3D.ProgressRatio >= 0.45 && pathFollow3D.ProgressRatio <= 0.55)
				{
					Item item = pathFollow3D.GetNode<Item>("Item");
					if (item != null)
					{
						ProcessItem(item);
					}
				}
			}
		}
	}

	public virtual void ProcessItem(Item item)
	{
		// Implementation for processing the item should go here
	}
}
