using Godot;
using System;
using System.IO;
using System.Linq;
public partial class Miner : Conveyor
{
	public ItemType itemTypeToSpawn { get; set; } = ItemType.Wood;
	public RayCast3D rayCast3D;

	public override void _Ready()
	{
		base._Ready();
		machineType = MachineType.Miner;
		rayCast3D = GetNode<RayCast3D>("RayCast3D");
	}


	public void Spawn()
	{
		if (rayCast3D.IsColliding())
		{
			var collider = rayCast3D.GetCollider();
			if (collider is ResourceNode)
			{
				ResourceNode resource = collider as ResourceNode;
				SpawnItem(resource.Resource);
			}
		}
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
