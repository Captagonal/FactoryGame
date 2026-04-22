using System;
using Godot;
using Godot.Collections;
public enum StoryProgress
{
	None,
	TutorialConveyor,
	TutorialMachine,
	BloblinNeedsHelp,
	BloblinNeedsHelp2,
	BloblinWantsSpace,
	
}
public partial class GameRunner : Node3D
{
	
	bool loadedResources = false;
	[Export] public PackedScene ConveyorScene;
	[Export] public PackedScene MachineScene;
	[Export] public PackedScene SpawnerScene;
	private const string SavePath = "user://savegame.save";
	// Called when the node enters the scene tree for the first time.
	Player player;
	private int Seed = 24232343;
	// public Task currentTask = new Task(ItemType.Wood, 5, Destination.Storage);
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.CurrentTask = new Task(ItemType.Wood, 5, Destination.Storage);;
		// 1. Get the Singleton
		var saveManager = GetNode<SaveManager>("/root/SaveManager");

		// 2. If we came from the "Load" button, run the load logic
		if (saveManager.PendingLoad)
		{
			saveManager.PendingLoad = false; // Reset the flag
			LoadGame();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("save_game"))
		{
			SaveGame();
		}
		else if (Input.IsActionJustPressed("load_game"))
		{
			LoadGame();
		}
	}

	public void ReturnToMenu()
	{
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}

	public void SaveGame()
	{
		using var saveFile = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);

		var saveData = new Dictionary();

		saveData[player.Name] = player.GlobalTransform;

		var conveyorData = new Godot.Collections.Array();
		var conveyors = GetTree().GetNodesInGroup("Conveyor");

		foreach (Node node in conveyors)
		{
			if (node is Conveyor conveyor) // Assuming your class is named Conveyor
			{
				MachineType type = conveyor.machineType;
				var dict = new Dictionary
				{
					{ "Pos", conveyor.GlobalPosition },
					{ "Rot", conveyor.GlobalRotation },
					{ "Type", (int)type },
				};
				conveyorData.Add(dict);
			}
		}
		saveData["Conveyors"] = conveyorData;

		saveData["Task"] = new Dictionary
		{
			{ "amount", player.CurrentTask.amount },
			{ "destination", (int)player.CurrentTask.destination },
			{ "itemType", (int)player.CurrentTask.itemType },
		};
		saveData["Inventory"] = player.inventory;
		saveData["seed"] = Seed;
		saveData["Progress"] = (int)player.progress;
		saveData["TasksDone"] = (int)player.completedTasks;
		saveFile.StoreVar(saveData);
	}
	public void LoadGame()
	{
		if (!FileAccess.FileExists(SavePath))
		{
			player.runConversation(ConversationGenerator.introConversation);
			Seed = unchecked((int)GD.Randi());
			placeResourceNodes();
			return;
		}

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		var saveData = (Dictionary)file.GetVar();
		Seed = (int)saveData["seed"];
		//get rid of old nodes
		string[] groups = { "Conveyor", "Machine", "Spawner" };
		foreach (string group in groups)
		{
			foreach (Node node in GetTree().GetNodesInGroup(group))
			{
				node.QueueFree();
			}
		}

		// Restore Player
		player.GlobalTransform = (Transform3D)saveData["Player"];

		foreach (Dictionary dict in (Godot.Collections.Array)saveData["Conveyors"])
		{
			MachineType type = (MachineType)(int)dict["Type"];
			switch (type)
			{
				case MachineType.Conveyor:
					MachineScene = GD.Load<PackedScene>("res://Scenes/Conveyor.tscn");
					break;
				case MachineType.Furnace:
					MachineScene = GD.Load<PackedScene>("res://Scenes/Funrace.tscn");
					break;
				case MachineType.Miner:
					MachineScene = GD.Load<PackedScene>("res://Scenes/Miner.tscn");
					break;
				case MachineType.Refiner:
					MachineScene = GD.Load<PackedScene>("res://Scenes/refiner.tscn");
					break;
			}
			var machine = MachineScene.Instantiate<Conveyor>();
			AddChild(machine);
			machine.GlobalPosition = (Vector3)dict["Pos"];
			machine.GlobalRotation = (Vector3)dict["Rot"];
			machine.machineType = type;
		}

		var taskDict = (Dictionary)saveData["Task"];
		player.CurrentTask = new Task((ItemType)(int)taskDict["itemType"], (int)taskDict["amount"], (Destination)(int)taskDict["destination"]);
		player.progress = (StoryProgress)(int)saveData["Progress"];
		player.inventory = (Dictionary<ItemType, int>)saveData["Inventory"];
		player.completedTasks = (int)saveData["TasksDone"];
		placeResourceNodes();
	}
	System.Collections.Generic.List<ItemType> itemTypesForNode = [
		ItemType.Wood,
		ItemType.Charcoal,
		ItemType.CopperOre,
		ItemType.IronOre,
	];
	private void placeResourceNodes()
	{
		if (loadedResources) return;
		PackedScene node = GD.Load<PackedScene>("res://Scenes/resource_node.tscn");
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		randomNumberGenerator.Seed = (ulong)Seed;
		int numberOfNodes = 1000;
		System.Collections.Generic.HashSet<Vector2> usedSpots =
		[
			new Vector2(0,0),
			new Vector2(0,1),
			new Vector2(1,0),
			new Vector2(1,1),
		];

		for (int i = 0; i < numberOfNodes; i++)
		{
			ResourceNode resourceNode = (ResourceNode)node.Instantiate();
			AddChild(resourceNode);
			resourceNode.setType(itemTypesForNode[(int)(randomNumberGenerator.Randi() % itemTypesForNode.Count)]);
			// GD.Print(resourceNode.Resource.ToString());
			int x = 0;
			int y = 0;
			int attempts = 0;
			while ((usedSpots.Contains(new Vector2(x,y)) && attempts < 100) || (Math.Abs(x) < 20 && Math.Abs(y) < 20)){
				x = randomNumberGenerator.RandiRange(-220,220);
				y = randomNumberGenerator.RandiRange(-220,220);
				attempts++;
			}
			usedSpots.Add(new Vector2I(x, y));
   			resourceNode.Position = new Vector3(x, 0, y);

		}
		loadedResources = true;
	}
	
	
}
