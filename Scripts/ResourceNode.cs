using Godot;
using System;

public partial class ResourceNode : Node3D
{
	[Export]
	public ItemType Resource {get; set;}
	public MeshInstance3D mesh;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mesh = GetNode<MeshInstance3D>("MeshInstance3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setType(ItemType type){
		Resource = type;
		// GD.Print("seting mat");
		StandardMaterial3D mat = new StandardMaterial3D();
		mat.AlbedoColor = Item.ItemToColor[type];
		// GD.Print(Item.ItemToColor[type]);
		mesh.SetSurfaceOverrideMaterial(0,mat);
		
	}
}
