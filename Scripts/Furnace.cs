using Godot;
using System;

public partial class Furnace : Conveyor
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		machineType = MachineType.Furnace;
		recipies.Add(new Recipie([ItemType.Wood], ItemType.Charcoal));
		recipies.Add(new Recipie([ItemType.IronOre], ItemType.Iron));
		recipies.Add(new Recipie([ItemType.CopperOre], ItemType.Copper));
	}
 

	public override void ProcessItem(Item item)
	{
		// GD.Print("Processing item of type: " + item.getType());
		base.ProcessItem(item);
		// Example processing logic: If the item is wood, turn it into charcoal
		foreach (var recipie in recipies)
		{
			if (recipie.CanProcess(item))
			{
				item.setType(recipie.output);
				break;
			}
		}
	}
}
