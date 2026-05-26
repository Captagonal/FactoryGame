using Godot;
using System;
using System.Linq;

public partial class Refiner : Conveyor
{
	// Called when the node enters the scene tree for the first time.
	public Path3D path2;

	public override void _Ready()
	{
		base._Ready();
		path2 = GetNode<Path3D>("Path3D2");

		machineType = MachineType.Refiner;
		recipies.Add(new Recipie([ItemType.Charcoal, ItemType.Iron], ItemType.Steel));
	}


	public override void ProcessItem(Item item)
	{
		base.ProcessItem(item);
		// Example processing logic: If the item is wood, turn it into charcoal
		if (item.getType() == ItemType.Charcoal && path.GetChildCount() > 1)
		{
			foreach (var child in path.GetChildren())
			{
				if (child is PathFollow3D pathFollow)
				{
					if (pathFollow.GetNode<Item>("Item").getType() == ItemType.Iron)
					{

						item.setType(ItemType.Steel);
						child.QueueFree();
						break;
					}
				}
			}
		}
	}
	public override void TakeInItem(Item item)
	{
		if (item.getType() == ItemType.Charcoal)
		{
			if (path == null)
			{
				path = GetNodeOrNull<Path3D>("Path3D");
			}
			PathFollow3D pathFollow3D = new PathFollow3D();
			pathFollow3D.Loop = false;
			pathFollow3D.RotationMode = PathFollow3D.RotationModeEnum.None; // Stops item from spinning
			path.AddChild(pathFollow3D);

			item.Reparent(pathFollow3D);
			item.Position = Vector3.Zero;
			pathFollow3D.ProgressRatio = 0;
		}
		else if (item.getType() == ItemType.Iron)
		{
			if (path2 == null)
			{
				path2 = GetNodeOrNull<Path3D>("Path3D2");
			}
			PathFollow3D pathFollow3D = new PathFollow3D();
			pathFollow3D.Loop = false;
			pathFollow3D.RotationMode = PathFollow3D.RotationModeEnum.None; // Stops item from spinning
			path2.AddChild(pathFollow3D);

			item.Reparent(pathFollow3D);
			item.Position = Vector3.Zero;
			pathFollow3D.ProgressRatio = 0;

		}
		else
		{
			item.QueueFree();
		}
	}

	public override void _Process(double delta)
	{
		if (path == null || path.GetChildCount() == 0) return;
		var items = path.GetChildren()
						 .OfType<PathFollow3D>()
						 .OrderByDescending(p => p.Progress)
						 .ToList();
		var items2 = path2.GetChildren()
						 .OfType<PathFollow3D>()
						 .OrderByDescending(p => p.Progress)
						 .ToList();

		
		for (int i = 0; i < items.Count; i++)
		{
			var current = items[i];
			Item item = current.GetNode<Item>("Item");



			item.Position = new Vector3(0, item.getSize() / 2 - .2f, 0);
			item.Rotation = Vector3.Zero; // Keep item from rotating


			float nextProgress = current.Progress + (speed * (float)delta);

			if (current.ProgressRatio > .4 && !item.isProcessed)
			{
				if (!TryProcess(item))
				{
					break;
				}
				ProcessItem(item);
				item.isProcessed = true;
			}
			if (i > 0)
			{
				var ahead = items[i - 1];
				if (nextProgress + current.GetNode<Item>("Item").getSize() >= ahead.Progress)
				{
					continue;
				}
			}
			if (nextProgress >= path.Curve.GetBakedLength())
			{
				TryTransferItem(current);
			}
			else
			{
				current.Progress = nextProgress;
			}
		}
		for (int i = 0; i < items2.Count; i++)
		{
			var current = items2[i];
			Item item = current.GetNode<Item>("Item");

			item.Position = new Vector3(0, item.getSize() / 2 - .2f, 0);
			item.Rotation = Vector3.Zero; // Keep item from rotating
			float nextProgress = current.Progress + (speed * (float)delta);
			if (current.ProgressRatio > .4 )
			{
				return;
			} else if (i > 0)
			{
				var ahead = items2[i - 1];
				if (nextProgress + current.GetNode<Item>("Item").getSize() >= ahead.Progress)
				{
					continue;
				}
			} else {
				current.Progress = nextProgress;
			}

		}
	}

	private bool TryProcess(Item item)
	{
		if (item.getType() == ItemType.Charcoal)
		{
			if (path2 == null || path2.GetChildCount() == 0) return false;
			foreach (var child in path2.GetChildren())
			{
				if (child is PathFollow3D pathFollow)
				{
					if (pathFollow.GetNode<Item>("Item").getType() == ItemType.Iron)
					{

						item.setType(ItemType.Steel);
						child.QueueFree();
						return true;
					}
				}
			}
		}
		return false;
	}

	public override bool canFit(float size, ItemType type){
		if ((path == null || path.GetChildCount() == 0) && (type == ItemType.Charcoal)) return true;
		if ((path2 == null || path2.GetChildCount() == 0) &&  type == ItemType.Iron) return true;
		if (path.GetChildCount() > 0 && type == ItemType.Charcoal)
		{
			var lastItem = path.GetChildren().OfType<PathFollow3D>().LastOrDefault();
			if (lastItem != null)
			{
				float lastItemEnd = lastItem.Progress + lastItem.GetNode<Item>("Item").getSize();
				if (lastItemEnd + size >= path.Curve.GetBakedLength())
				{
					return false;
				}
			}
		}
		if (path2.GetChildCount() > 0 && type == ItemType.Iron)
		{
			var lastItem = path2.GetChildren().OfType<PathFollow3D>().LastOrDefault();
			if (lastItem != null)
			{
				float lastItemEnd = lastItem.Progress + lastItem.GetNode<Item>("Item").getSize();
				if (lastItemEnd + size >= path2.Curve.GetBakedLength())
				{
					return false;
				}
			}
		}
		return true;
	}
}
