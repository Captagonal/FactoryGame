using Godot;
using System;

public partial class Pause : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ReturnToGame()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		GetTree().Paused = false;
		Visible = false;
	}
	public void quit()
	{
		GameRunner gameRunner = (GameRunner)GetParent().GetParent();
		gameRunner.SaveGame();
		GetTree().CreateTimer(1f).Timeout += () =>
		{
			GetTree().Quit();
		};
	}
}
