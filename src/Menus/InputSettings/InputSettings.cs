using Godot;
using System;
using Godot.Collections;

public partial class InputSettings : Control
{
	[Export] private PackedScene _inputButtonScene;
	private VBoxContainer _actionList;
	private bool _isRemapping = false;
	private object _actionToRemap = null;

	private object remappingButton = null;

	public Dictionary<String, String> inputAction = new Dictionary<string, string>()
	{
		{ "click", "Shoot" },
		{ "move_left", "Left" },
		{ "move_right", "Right" },
		{ "jump", "Jump" },
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_actionList = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/ActionList");
		_createActionList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _createActionList()
	{
		InputMap.LoadFromProjectSettings();
		foreach (var item in _actionList.GetChildren())
		{
			item.QueueFree();
		}

		InputMap.GetActions();
		foreach (var action in inputAction.Keys)
		{
			GD.Print("Action triggered:" + action);
			var button = _inputButtonScene.Instantiate<Node>();
			var actionLabel = button.FindChild("LabelAction");
			var inputLabel = button.FindChild("LabelInput");

			actionLabel.Set("text", inputAction[action]);

			var events = InputMap.ActionGetEvents(action);
			if (events.Count > 0)
			{
				inputLabel.Set("text", events[0].AsText().TrimSuffix(" (Physical)"));
			}
			else
			{
				inputLabel.Set("text", "Not set");
			}

			_actionList.AddChild(button);
			//TODO: Connect the button to the input_button_pressed signal
			button.Connect("pressed", new Callable(this, "_on_input_button_pressed"));

			var exitButton = _inputButtonScene.Instantiate<Node>();
			var exitActionLabel = exitButton.FindChild("LabelAction");
			exitActionLabel.Set("text", "Exit");
		}
	}

	private void _on_input_button_pressed(Node button, String action)
		{
			if (!_isRemapping)
			{
				_isRemapping = true;
				_actionToRemap = action;
				remappingButton = button;
				button.FindChild("LabelInput").Set("text", "Press key to bind...");
			}
		}
}
