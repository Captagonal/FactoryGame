using Godot;
using System;
using System.IO;
using System.Linq;

public partial class Spawner : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	public Path3D path;
	private float speed = 1f;
	private Area3D Out, In;
	private Conveyor nextConveyor;
	public override void _Ready()
	{
		path = GetNode<Path3D>("Path3D");
		// Item item = Item.newItem(ItemType.Wood);
		// AddChild(item);
		// TakeInItem(item);
		// Item item1 = Item.newItem(ItemType.Charcoal);
		// AddChild(item1);
		// TakeInItem(item1);
		Out = GetNode<Area3D>("Out");
		In = GetNode<Area3D>("In");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (path == null || path.GetChildCount() == 0) return;
		for (int i = 0; i < path.GetChildCount(); i++)
		{
			Node child = path.GetChild(i);
			if (child is PathFollow3D pathFollow3D)
			{
				pathFollow3D.ProgressRatio += (float)delta * speed;
				if (pathFollow3D.ProgressRatio >= .98)
				{
					// nextConveyor = Out.GetOverlappingAreas().FirstOrDefault().GetParent() as Conveyor;
					// GD.Print("Next Conveyor: " + nextConveyor.Name);
					// pathFollow3D.ProgressRatio = 0;
					// nextConveyor.takeInItem(pathFollow3D.GetNode<Item>("Item"));

					// pathFollow3D.QueueFree();
					TryTransferItem(pathFollow3D);
				}
			}
		}
	}

public void Spawn(){
	SpawnItem(ItemType.Wood);
}
	public void SpawnItem(ItemType type)
	{
		Item newItem = Item.newItem(type);
		AddChild(newItem);
		TakeInItem(newItem);
	}
private void TryTransferItem(PathFollow3D pathFollow)
{
	// 1. Get the first overlapping area safely
	if (Out == null|| Out.GetOverlappingAreas().Count == 0)
	{
		return;
	}
	
	var nextArea = Out.GetOverlappingAreas().FirstOrDefault();
	
	// 2. Check if the area exists AND its parent is a Conveyor
	// This 'is' check handles the null check and type check at once
	if (nextArea?.GetParent() is Conveyor targetConveyor)
	{
		Item item = pathFollow.GetNodeOrNull<Item>("Item");
		
		if (item != null)
		{
			targetConveyor.TakeInItem(item);
			pathFollow.QueueFree();
		}
	}
	else
	{
		// If we reach here, either:
		// - Nothing is touching the 'Out' area
		// - Something is touching it, but it's a Furnace/Machine that isn't a 'Conveyor'
		
		// Stop the item at the edge so it doesn't disappear/loop
		pathFollow.ProgressRatio = 0.99f;
		
		// DEBUG: Uncomment the line below to see what is actually being hit
		// if (nextArea != null) GD.Print("Hit something not a conveyor: " + nextArea.GetParent().Name);
	}
}
	public void TakeInItem(Item item)
{
	// 1. Safety check: If path is still null, try to find it again
	if (path == null)
	{
		path = GetNodeOrNull<Path3D>("Path3D");
	}

	// 2. Final Null Check
	if (path == null)
	{
		GD.PrintErr($"Critical: {Name} is missing a Path3D node!");
		return;
	}

	// 3. Create the carrier
	PathFollow3D pathFollow3D = new PathFollow3D();
	pathFollow3D.Loop = false;
	
	path.AddChild(pathFollow3D);

	// 4. Handle Item Reparenting
	if (item != null)
	{
		item.Reparent(pathFollow3D);
		item.Position = Vector3.Zero;
		pathFollow3D.ProgressRatio = 0;
	}
}

	public void getNext()
	{

	}

}
