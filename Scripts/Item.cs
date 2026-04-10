using Godot;
using System;

public partial class Item : RigidBody3D
{
	private ItemType type;
	private float size = 0.5f;
	public static Item newItem(ItemType type, float size = 0.5f)
	{
		PackedScene scene = GD.Load<PackedScene>("res://Scenes/Item.tscn");
		Item item = scene.Instantiate<Item>();
		item.setType(type);	
		item.setSize(size);
		item.Name = "Item"	;
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
		MeshInstance3D meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		switch (type)
		{
			case ItemType.Wood:
				meshInstance.Mesh = GD.Load<Mesh>("res://Models/WoodMesh.tres");
				break;
			case ItemType.Stone:
				meshInstance.Mesh = GD.Load<Mesh>("res://Models/StoneMesh.tres");
				break;
			case ItemType.Iron:
				meshInstance.Mesh = GD.Load<Mesh>("res://Models/IronMesh.tres");
				break;
			case ItemType.Copper:
				meshInstance.Mesh = GD.Load<Mesh>("res://Models/CopperMesh.tres");
				break;
			case ItemType.Charcoal:
				meshInstance.Mesh = GD.Load<Mesh>("res://Models/CharcoalMesh.tres");
				break;
		}
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
		size = newSize;	}
}

public enum ItemType
{
	Wood,
	Stone,
	Iron,
	Copper,
	Charcoal
}
