using System;
using Godot;
using Godot.Collections;

public partial class Player : Node3D
{
	// Called when the node enters the scene tree for the first time.
	float CameraSensitivity = 0.005f;
	private Vector3 _grabPointWorld;
	private Vector3 _cameraStartPos;
	private Node3D pointer;
	private Control InventoryUI, BuildUI, Hud;
	private Label TaskView;
	private bool _isPlacingConveyor = false;
	private bool BuildMode, Inventory = false;
	public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
	private Conversation conversation;
	public Task CurrentTask;
	public override void _Ready()
	{
		pointer = GetNode<Node3D>("pointer");
		InventoryUI = GetNode<Control>("Inventory");
		BuildUI = GetNode<Control>("BuildUI");
		Hud = GetNode<Control>("HUD");
		conversation = Hud.GetNode<Conversation>("Conversation");
		TaskView = Hud.GetNode<Label>("Current Task");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Camera3D camera3D = GetNode<Camera3D>("Camera3D");

		// GD.Print(Position);
		// GD.Print(RotationDegrees);
		// GD.Print(camera3D.RotationDegrees);	
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
			else
			{
				GD.Print("rot point");
				GD.Print(pointer.RotationDegrees);
				pointer.RotateY(Mathf.DegToRad(90));
			}
		}
		if (Input.IsActionJustPressed("Remove"))
		{
			if (isPointerConveyor())
			{
				targetedConveyor().QueueFree();
			}
		}
		if (Input.IsActionJustPressed("BuildMode"))
		{
			BuildMode = !BuildMode;
			if (BuildMode)
			{
				Inventory = false;
			}
			else
			{
				building = BuildMode;
				// TODO: Clear pointer status back to default
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
		if (Input.IsActionJustPressed("Test"))
		{
			completedTasks = 25;
			completeTask();
		}

		BuildUI.Visible = BuildMode;

		InventoryUI.Visible = Inventory;
		Hud.Visible = (!BuildMode && !Inventory);

		updateTaskVeiw();

		if (building)
		{

			// PackedScene machineScene = scenes[buildType];
			// StandardMaterial3D material3D = new StandardMaterial3D();
			// material3D.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			// material3D.AlbedoColor = new Color(20, 20, 50, 100);
			// var pointerMesh = pointer.GetNode<MeshInstance3D>("Torus");
			// pointerMesh.Visible = false;
			// // pointer.GetNode<MeshInstance3D>("Torus").MaterialOverride = material3D;
			// var placeholder = machineScene.Instantiate<Conveyor>();
			// pointer.AddChild(placeholder);



			if (Input.IsActionJustPressed("Accept") && !IsPointerOverUI())
			{
				if (isPointerConveyor())
				{
					return;
				}
				PackedScene machineScene = scenes[buildType];
				building = BuildMode;
				// placeholder.QueueFree();
				if (machineScene != null)
				{
					var MachineInstance = machineScene.Instantiate<Conveyor>();
					GetParent().AddChild(MachineInstance);
					MachineInstance.GlobalPosition = new Vector3(Mathf.Round(pointer.GlobalPosition.X), 0.4f, Mathf.Round(pointer.GlobalPosition.Z));
					MachineInstance.RotationDegrees = new Vector3(0, pointer.RotationDegrees.Y, 0);
				}
			}
		}

	}
	private static Dictionary<MachineType, PackedScene> scenes = new Dictionary<MachineType, PackedScene>{
		{MachineType.Conveyor, GD.Load<PackedScene>("res://Scenes/Conveyor.tscn")},
		{MachineType.Furnace, GD.Load<PackedScene>("res://Scenes/Funrace.tscn")},
		{MachineType.Miner, GD.Load<PackedScene>("res://Scenes/Miner.tscn")},
		{MachineType.Refiner, GD.Load<PackedScene>("res://Scenes/refiner.tscn")},
	};

	public void updateTaskVeiw()
	{
		TaskView.Text = "Current Task:\nDeliver " + +CurrentTask.amount + " " + CurrentTask.itemType.ToString() + " to " + CurrentTask.destination;
	}

	public void runConversation(System.Collections.Generic.Dictionary<string, Texture2D> dict)
	{
		conversation.start(dict);
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
				if (isOutOfBounds(_cameraStartPos + difference))
				{

				}
				else
				{
					GlobalPosition = _cameraStartPos + difference;


					_cameraStartPos = GlobalPosition;
				}

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
		buildType = type;
		building = true;
		//TODO: Set pointer to be a transparent version of the mesh to place down
	}

	public void addToInventory(Item item)
	{
		if (inventory.ContainsKey(item.getType()))
		{
			inventory[item.getType()] += 1;
		}
		else
		{
			inventory[item.getType()] = 1;
		}
		if (CurrentTask.itemType == item.getType())
		{
			CurrentTask.ProcessTask();
			if (CurrentTask.TaskCompleted())
			{
				completeTask();
			}
		}
	}
	public int completedTasks = 0;
	public StoryProgress progress = StoryProgress.None;
	private void completeTask()
	{
		CurrentTask = new Task(progress);
		completedTasks++;
		if (completedTasks > 25)
		{
			progress = StoryProgress.BloblinWantsSpace;
			//END GAME
			// (4.5938373, 3.192306, 3.3522)
			// (-0, 56.587936, 0)
			// (-21.485928, 0, 0)

			Camera3D camera3D = GetNode<Camera3D>("Camera3D");

			conversation.start(ConversationGenerator.end);
			Tween tween = GetTree().CreateTween();
			Tween tween1 = GetTree().CreateTween();
			Tween tween2 = GetTree().CreateTween();
			tween.TweenProperty(this, "position", new Vector3(4.5938373f, 3.192306f, 3.3522f), 5);
			tween1.TweenProperty(this, "rotation_degrees", new Vector3(0, 56.587936f, 0), 5);
			tween2.TweenProperty(camera3D, "rotation_degrees", new Vector3(-21.485928f, 0, 0), 5);
			SceneTreeTimer timer = GetTree().CreateTimer(5);
			timer.Timeout += () =>
			{
				Tween RocketTween = GetTree().CreateTween();
				RocketTween.TweenProperty(GetNode<Node3D>("%Rocket"), "position", new Vector3(4.5938373f, 100, 3.3522f), 15);

				GetTree().CreateTimer(1).Timeout += () =>
				{
					Tween playerTween = GetTree().CreateTween();
					playerTween.TweenProperty(this, "position", new Vector3(20, 100, 3.3522f), 16);
				};
				GetTree().CreateTimer(13).Timeout += () =>
				{
					GetTree().Quit();
				};
			};
			return;
		}
		if (completedTasks > 20)
		{
			progress = StoryProgress.BloblinNeedsHelp2;
			conversation.start(ConversationGenerator.ch3Conversation);
			return;
		}
		if (completedTasks > 15)
		{
			progress = StoryProgress.BloblinNeedsHelp;
			conversation.start(ConversationGenerator.ch2Conversation);
			return;
		}
		if (completedTasks > 10)
		{
			progress = StoryProgress.TutorialMachine;
			conversation.start(ConversationGenerator.tut3);

			return;
		}
		if (completedTasks > 5)
		{
			progress = StoryProgress.TutorialConveyor;
			conversation.start(ConversationGenerator.tut2);

			return;
		}

	}
	private bool isOutOfBounds(Vector3 pos)
	{
		if (Math.Abs(pos.X) > 220)
		{
			return true;
		}
		if (Math.Abs(pos.Z) > 220)
		{
			return true;
		}
		return false;
	}
}
