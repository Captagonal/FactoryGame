using Godot;
using Godot.Collections;
using System;

public partial class Item : Node3D
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
	public static Dictionary<ItemType, Color> ItemToColor = new Dictionary<ItemType, Color>{
		{ItemType.Charcoal, Colors.Black},
		{ItemType.Copper, Colors.Orange},
		{ItemType.CopperOre, Colors.Orange},
		{ItemType.Iron, Colors.Silver},
		{ItemType.IronOre, Colors.Silver},
		{ItemType.Wood, Colors.SaddleBrown},
		{ItemType.Steel, Colors.DarkGray},
		{ItemType.Stone, Colors.Gray},
	};
	public static Dictionary<ItemType, PackedScene> ItemToModel = new Dictionary<ItemType, PackedScene>{
		{ItemType.Charcoal, GD.Load<PackedScene>("res://Models/Coal.blend")},
		{ItemType.Copper, GD.Load<PackedScene>("res://Models/Copper.blend")},
		{ItemType.CopperOre, GD.Load<PackedScene>("res://Models/CopperOre.blend")},
		{ItemType.Iron, GD.Load<PackedScene>("res://Models/Iron.blend")},
		{ItemType.IronOre, GD.Load<PackedScene>("res://Models/IronOre.blend")},
		{ItemType.Wood, GD.Load<PackedScene>("res://Models/Wood.blend")},
		{ItemType.Steel, GD.Load<PackedScene>("res://Models/Steel.blend")},
		{ItemType.Stone, GD.Load<PackedScene>("res://Models/Stone.blend")},
	};
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
			if (child.IsInGroup("Model"))
			{
				child.QueueFree();
			}
		}

		Node3D node = null;

		node = ItemToModel[newType].Instantiate<Node3D>();
		node.Scale = new Vector3(.1f, .1f, .1f);
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
