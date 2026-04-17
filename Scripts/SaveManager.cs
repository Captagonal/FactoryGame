using Godot;

public partial class SaveManager : Node
{
	// A flag to tell the GameRunner "Hey, we are starting from a load!"
	public bool PendingLoad = false;

	public void LoadGameFromMenu()
	{
		PendingLoad = true;
		// This triggers the scene change
		GetTree().ChangeSceneToFile("res://root.tscn");
	}
}
