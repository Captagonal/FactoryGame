using Godot;
using System;

public partial class Furnace : Machine
{
	// Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// 	base._Ready();
	// }
 

	public override void ProcessItem(Item item)
	{
		GD.Print("Processing item of type: " + item.getType());
		base.ProcessItem(item);
		// Example processing logic: If the item is wood, turn it into charcoal
		if (item.getType() == ItemType.Wood)
		{
			item.setType(ItemType.Charcoal);
		
			GD.Print("Processed wood into charcoal!");
			// You can add more processing logic for other item types here
		}
	}
}
