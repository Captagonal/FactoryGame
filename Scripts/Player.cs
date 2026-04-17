using Godot;
using Godot.Collections;

public partial class Player : Node3D
{
	// Called when the node enters the scene tree for the first time.
	float CameraSensitivity = 0.005f;
	private Vector3 _grabPointWorld;
	private Vector3 _cameraStartPos;
	private Node3D pointer;
	private Control InventoryUI, BuildUI;
	private bool _isPlacingConveyor = false;
	private bool BuildMode, Inventory = false;
	public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
	public override void _Ready()
	{
		pointer = GetNode<Node3D>("pointer");
		InventoryUI = GetNode<Control>("Inventory");
		BuildUI = GetNode<Control>("BuildUI");
		inventory.Add(ItemType.Wood, 50);
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
		if (IsPointerOverUI())
		{
			pointer.Visible = false;
		}
		else
		{
			pointer.Visible = true;
		}

		if (Input.IsActionJustPressed("RotateObject"))
		{
			if (isPointerConveyor())
			{
				targetedConveyor().RotateY(Mathf.DegToRad(90));
			}
		}
		if (Input.IsActionJustPressed("BuildMode"))
		{
			BuildMode = !BuildMode;
			if (BuildMode)
			{
				Inventory = false;
			}
		}
		if (Input.IsActionJustPressed("Inventory"))
		{
			Inventory = !Inventory;
			if (Inventory)
			{
				BuildMode = false;
				InventoryUI.GetNode<ItemList>("ItemList").Clear();
				foreach (var item in inventory)
				{
					InventoryUI.GetNode<ItemList>("ItemList").AddItem(item.Key.ToString() + ": " + item.Value);
				}
			}
		}

		BuildUI.Visible = BuildMode;
		InventoryUI.Visible = Inventory;

		if (building)
		{

			if (Input.IsActionJustPressed("Accept") && !IsPointerOverUI())
			{
				PackedScene machineScene = null;
				switch (buildType)
				{
					case MachineType.Conveyor:
						machineScene = GD.Load<PackedScene>("res://Scenes/Conveyor.tscn");
						break;
					case MachineType.Furnace:
						machineScene = GD.Load<PackedScene>("res://Scenes/Funrace.tscn");
						break;
				}
				if (machineScene != null)
				{
					var MachineInstance = machineScene.Instantiate<Conveyor>();
					GetParent().AddChild(MachineInstance);
					MachineInstance.GlobalPosition = new Vector3(Mathf.Round(pointer.GlobalPosition.X), 0.4f, Mathf.Round(pointer.GlobalPosition.Z));
				}
			}
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
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.IsPressed() && mouseButton.ButtonIndex == MouseButton.WheelUp)
			{
				// zoom in
				GlobalPosition -= camera3D.GlobalTransform.Basis.Z;
			}
			else if (mouseButton.IsPressed() && mouseButton.ButtonIndex == MouseButton.WheelDown)
			{
				GlobalPosition += camera3D.GlobalTransform.Basis.Z;
			}
			if (GlobalPosition.Y < 0)
			{
				GlobalPosition = new Vector3(GlobalPosition.X, 0, GlobalPosition.Z);
			}
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
		if (groundPlane.IntersectsRay(rayOrigin, rayNormal) == null)
		{
			return pointer.GlobalPosition; // Avoid division by zero
		}
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
		// return false;
		if (GetViewport() == null)
		{
			return false;
		}
		Vector2 mousePos = GetViewport().GetMousePosition();
		var uiElements = GetTree().GetNodesInGroup("UI");
		foreach (var element in uiElements)
		{
			// GD.Print(element.Name);
			if (element is Control control && control.GetGlobalRect().HasPoint(mousePos) && control.IsVisibleInTree())
			{
				// GD.Print("Pointer is over UI element: " + control.Name);
				return true;
			}
		}
		return false;
	}

	private bool isPointerConveyor()
	{
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
	private Conveyor targetedConveyor()
	{
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
	bool building = false;
	MachineType buildType = MachineType.Conveyor;
	public void Build(MachineType type)
	{
		// GD.Print("Building " + type);
		building = true;
		buildType = type;
	}
}
