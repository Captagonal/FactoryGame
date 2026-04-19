using Godot;
using System;

public partial class Item : RigidBody3D
{
	private ItemType type;
	private float size = 0.5f;
	public bool isProcessed = false;
	public static Item newItem(ItemType type, float size = 0.5f)
	{
		PackedScene scene = GD.Load<PackedScene>("res://Scenes/Item.tscn");
		Item item = scene.Instantiate<Item>();
		item.setType(type);
		item.setSize(size);
		item.Name = "Item";
		return item;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void setType(ItemType newType)
	{
		type = newType;
		foreach (var child in GetChildren())
		{
			if (child.IsInGroup("Model")){
				child.QueueFree();
			}
		}
		// MeshInstance3D meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		// switch (type)
		// {
		// 	case ItemType.Wood:
		// 		meshInstance.Mesh = GD.Load<Mesh>("res://Models/WoodMesh.tres");
		// 		break;
		// 	case ItemType.Stone:
		// 		meshInstance.Mesh = GD.Load<Mesh>("res://Models/StoneMesh.tres");
		// 		break;
		// 	case ItemType.Iron:
		// 		meshInstance.Mesh = GD.Load<Mesh>("res://Models/IronMesh.tres");
		// 		break;
		// 	case ItemType.Copper:
		// 		meshInstance.Mesh = GD.Load<Mesh>("res://Models/CopperMesh.tres");
		// 		break;
		// 	case ItemType.Charcoal:
		// 		meshInstance.Mesh = GD.Load<Mesh>("res://Models/Coal.blend");
		// 		break;
		// }
		Node3D node = null;
		switch (type)
		{
			case ItemType.Wood:
				node = (Node3D)GD.Load<PackedScene>("res://Models/Wood.blend").Instantiate();
				break;
			case ItemType.Charcoal:
				node = (Node3D)GD.Load<PackedScene>("res://Models/Coal.blend").Instantiate();
				break;
			case ItemType.Copper:
				node = (Node3D)GD.Load<PackedScene>("res://Models/Copper.blend").Instantiate();
				break;
			case ItemType.CopperOre:
				node = (Node3D)GD.Load<PackedScene>("res://Models/CopperOre.blend").Instantiate();
				break;
			case ItemType.Iron:
				node = (Node3D)GD.Load<PackedScene>("res://Models/Iron.blend").Instantiate();
				break;
			case ItemType.IronOre:
				node = (Node3D)GD.Load<PackedScene>("res://Models/IronOre.blend").Instantiate();
				break;
			case ItemType.Steel:
				node = (Node3D)GD.Load<PackedScene>("res://Models/Steel.blend").Instantiate();
				break;

		}
		node.Scale = new Vector3(.1f,.1f,.1f);
		AddChild(node);
		node.AddToGroup("Model");
	}
	public ItemType getType()
	{
		return type;
	}
	public float getSize()
	{
		return size;
	}
	public void setSize(float newSize)
	{
		size = newSize;
	}
}

public enum ItemType
{
	Wood,
	Stone,
	Iron,
	IronOre,
	Copper,
	CopperOre,
	Charcoal,
	Steel
}
