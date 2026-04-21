using Godot;

public partial class SaveManager : Node
{
	public bool PendingLoad = false;

	public void LoadGameFromMenu()
	{
		PendingLoad = true;
		// This triggers the scene change
		GetTree().ChangeSceneToFile("res://root.tscn");
	}
}
