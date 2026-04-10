using Godot;
using System;
using System.IO;
using System.Linq;

public partial class Spawner : Conveyor
{
	public void Spawn()
	{
		SpawnItem(ItemType.Wood);
	}
	public void SpawnItem(ItemType type)
	{
		if (path == null)
		{
			return;
		}
		if (path.GetChildCount() == 0)
		{
			Item newItem = Item.newItem(type);
			AddChild(newItem);
			TakeInItem(newItem);
		}
		var items = path.GetChildren()
						 .OfType<PathFollow3D>()
						 .OrderByDescending(p => p.Progress)
						 .Reverse()
						 .ToList();

		if (items.FirstOrDefault()?.Progress > 0.5f)
		{
			Item newItem = Item.newItem(type);
			AddChild(newItem);
			TakeInItem(newItem);
		}
		
	}

}
