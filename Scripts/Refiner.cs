using Godot;
using System;

public partial class Refiner : Conveyor
{
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		base._Ready();
		machineType = MachineType.Refiner;
		recipies.Add(new Recipie([ItemType.Charcoal, ItemType.Iron], ItemType.Steel));
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
