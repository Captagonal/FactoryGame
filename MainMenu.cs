using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void LoadGame()
	{
		GD.Print("LoadGame called in MainMenu");
		var saveManager = GetNode<SaveManager>("/root/SaveManager");
		saveManager.LoadGameFromMenu();
	}
	private void ApplyLoadData()
	{
		// Find the GameRunner in the n	ewly loaded scene
		if (GetTree().CurrentScene is GameRunner gameRunner)
		{
			gameRunner.LoadGame();
		}
		else
		{
			GD.PrintErr("Error: The loaded scene's root node does not have the GameRunner script!");
		}
	}
}
