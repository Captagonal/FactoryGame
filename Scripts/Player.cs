using Godot;
using System;
using System.IO;

public partial class Player : Node3D
{
	// Called when the node enters the scene tree for the first time.
	float CameraSensitivity = 0.005f;
	private Vector3 _grabPointWorld;
	private Vector3 _cameraStartPos;
	private Node3D pointer;
	private bool _isPlacingConveyor = false;
	public override void _Ready()
	{
		pointer = GetNode<Node3D>("pointer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("rotate") && !Input.IsKeyPressed(Key.Shift))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		pointer.GlobalPosition = getMousePointerPosition();
		if (IsPointerOverUI()){
			pointer.Visible = false;
		}
		else
		{
			pointer.Visible = true;
		}
		
		if (isPointerConveyor()){
			GD.Print("Hovering over conveyor"+ targetedConveyor().Name);
			
		}
	}
	public override void _Input(InputEvent @event)
	{

		Camera3D camera3D = GetNode<Camera3D>("Camera3D");

		if (@event.IsActionPressed("Move"))
		{
			_grabPointWorld = GetMouseWorldPosition(GetViewport().GetMousePosition());
			_cameraStartPos = GlobalPosition;
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (Input.IsActionPressed("Move"))
			{
				Vector3 currentMousePos = GetMouseWorldPosition(mouseMotion.Position);

				Vector3 difference = _grabPointWorld - currentMousePos;
				GlobalPosition = _cameraStartPos + difference;

				_cameraStartPos = GlobalPosition;
			}
			else if (Input.IsActionPressed("rotate"))
			{
				this.RotateY(-mouseMotion.Relative.X * CameraSensitivity);
				camera3D.RotateX(-mouseMotion.Relative.Y * CameraSensitivity);
				var camRotation = camera3D.Rotation;
				camRotation.X = Mathf.Clamp(camRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
				camera3D.Rotation = camRotation;
			}
		}
		else if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape)
		{


			Input.MouseMode = Input.MouseModeEnum.Visible;
			// GetParent().GetNode<Control>("Settings").Visible = true;
			GetTree().Paused = true;

		}
	}
	private Vector3 GetMouseWorldPosition(Vector2 mousePos)
	{
		Camera3D camera3D = GetNode<Camera3D>("Camera3D");

		Plane groundPlane = new Plane(Vector3.Up, 0);

		Vector3 rayOrigin = camera3D.ProjectRayOrigin(mousePos);
		Vector3 rayNormal = camera3D.ProjectRayNormal(mousePos);

		Vector3 intersection = (Vector3)groundPlane.IntersectsRay(rayOrigin, rayNormal);

		return intersection;
	}
	private Vector3 getMousePointerPosition()
	{
		if (GetViewport() == null)
		{
			return pointer.GlobalPosition;
		}
		Vector3 worldPoint = GetMouseWorldPosition(GetViewport().GetMousePosition());
		return new Vector3(Mathf.Round(worldPoint.X), 0, Mathf.Round(worldPoint.Z));
	}

	private bool IsPointerOverUI()
	{
		return false;
		// if (GetViewport() == null)
		// {
		// 	return false;
		// }
		// Vector2 mousePos = GetViewport().GetMousePosition();
		// var uiElements = GetTree().GetNodesInGroup("UI");
		// foreach (var element in uiElements)
		// {
		// 	if (element is Control control && control.GetGlobalRect().HasPoint(mousePos))
		// 	{
		// 		return true;
		// 	}
		// }
		// return false;
	}

	private bool isPointerConveyor(){
		RayCast3D rayCast3D = GetNode<RayCast3D>("pointer/RayCast3D");
		if (rayCast3D.IsColliding())
		{
			var collider = rayCast3D.GetCollider();
			if (collider is Conveyor)
			{
				return true;
			}
		}
		return false;
	}
	private Conveyor targetedConveyor(){
		RayCast3D rayCast3D = GetNode<RayCast3D>("pointer/RayCast3D");
		if (rayCast3D.IsColliding())
		{
			var collider = rayCast3D.GetCollider();
			if (collider is Conveyor conveyor)
			{
				return conveyor;
			}
		}
		return null;
	}
}
