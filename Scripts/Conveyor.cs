using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Conveyor : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	public MachineType machineType = MachineType.Conveyor;

	public Path3D path;
	private float speed = 1f;
	private Area3D Out, In;
	private Conveyor nextConveyor;

	// public void create(Path3D newPath)
	// {
	// 	path = GetNode<Path3D>("Path3D");
	// 	AddChild(newPath);
	// 	path.QueueFree();
	// 	newPath.Name = "Path3D";
	// 	this.path = newPath;

	// 	Out.Position = new Vector3(path.Curve.GetBakedPoints().Last().X, 0, path.Curve.GetBakedPoints().Last().Z);
	// 	In.Position = new Vector3(path.Curve.GetBakedPoints().First().X, 0, path.Curve.GetBakedPoints().First().Z);

	// 	csgPolygon.PathNode = path.GetPathTo(newPath);
	// }

	public override void _Ready()
	{
		path = GetNode<Path3D>("Path3D");
		// csgPolygon = GetNode<CsgPolygon3D>("Conveyor");
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
		var items = path.GetChildren()
						 .OfType<PathFollow3D>()
						 .OrderByDescending(p => p.Progress)
						 .ToList();


		for (int i = 0; i < items.Count; i++)
		{
			var current = items[i];
			Item item = current.GetNode<Item>("Item");

		

			item.Position = new Vector3(0, item.getSize() / 2, 0);
			item.Rotation = Vector3.Zero; // Keep item from rotating


			float nextProgress = current.Progress + (speed * (float)delta);

			if (current.ProgressRatio > .4 && !item.isProcessed)
			{
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
	}

	private void TryTransferItem(PathFollow3D pathFollow)
	{
		Item item = pathFollow.GetNodeOrNull<Item>("Item");
		if (item == null)
		{
			return;
		}

		// 1. Get the first overlapping area safely
		if (Out == null || Out.GetOverlappingAreas().Count == 0)
		{
			return;
		}

		var nextArea = Out.GetOverlappingAreas().FirstOrDefault();
		if (nextArea?.GetParent() is Conveyor targetConveyor && targetConveyor.canFit(item.getSize()))
		{
			// Transfer to next conveyor
			targetConveyor.TakeInItem(item);
			item.isProcessed = false; // Reset processed state for the next conveyor
			pathFollow.QueueFree();
		}
		else
		{
			// Blocked at the end of the belt
			pathFollow.ProgressRatio = 1.0f;
		}
	}
	public bool canFit(float size)
	{
		var firstItem = path.GetChildren()
							 .OfType<PathFollow3D>()
							 .OrderBy(p => p.Progress)
							 .FirstOrDefault();

		if (firstItem == null) return true;
		return firstItem.Progress > size;

	}
	public void TakeInItem(Item item)
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
	public virtual void ProcessItem(Item item)
	{
		// Implementation for processing the item should go here
	}

	public List<Item> GetItemsOnBelt()
	{
		return path.GetChildren()
				   .OfType<PathFollow3D>()
				   .Select(pf => pf.GetNode<Item>("Item"))
				   .ToList();
	}

	public List<Recipie> recipies = new List<Recipie>();


}
public enum MachineType
{
	Conveyor,
	Furnace,
	Assembler,
	Crafter
}
