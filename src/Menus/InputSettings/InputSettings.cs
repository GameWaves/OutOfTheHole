using Godot;
using System;
using System.Diagnostics;
using Godot.Collections;

public partial class InputSettings : Control
{
	[Export] private PackedScene _inputButtonScene;
	[Export] private PackedScene _mainMenuScene;
	private VBoxContainer _actionList;
	private bool _isRemapping = false;
	private StringName _actionToRemap = null;

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
			var exitButton = _inputButtonScene.Instantiate<Node>();
			var exitActionLabel = exitButton.FindChild("LabelAction");
			exitActionLabel.Set("text", "Exit");
			((Button)button).Pressed += () => _on_input_button_pressed(button, action);
		}
	}

	private void _on_input_button_pressed(Node button, String action)
	{
		//TODO: Add focus to the button
		((Button)button).FocusMode = FocusModeEnum.All;
		((Button)button).GrabFocus();
		if (!_isRemapping)
		{
			_isRemapping = true;
			_actionToRemap = action;
			remappingButton = button;
			button.FindChild("LabelInput").Set("text", "Press key to bind...");
		}
	}

	public override void _Input(InputEvent eventKey)
	{
		if (_isRemapping)
		{
			if (eventKey is InputEventKey key)
			{
				Debug.Print("Remapping action: " + _actionToRemap + " to key: " + eventKey);
				InputMap.ActionEraseEvents(_actionToRemap);
				InputMap.ActionAddEvent(_actionToRemap, (InputEventKey) eventKey);
				_updateActionList((Node) remappingButton, key.KeyLabel.ToString());
				_isRemapping = false;
				_actionToRemap = null;
				remappingButton = null;
			}
			else if (eventKey is InputEventMouseButton mouseButton && mouseButton.Pressed)
			{
				Console.WriteLine("Remapping action: " + _actionToRemap + " to key: " + eventKey);
				InputMap.ActionEraseEvents(_actionToRemap);
				InputMap.ActionAddEvent(_actionToRemap, (InputEventMouseButton) eventKey);
				_updateActionList((Node)remappingButton, "Mouse " + mouseButton.ButtonIndex);
				_isRemapping = false;
				_actionToRemap = null;
				remappingButton = null;
			}
			else
			{
				Console.WriteLine("Invalid input event type: " + eventKey);
			}
		}
	}

	private void _updateActionList(Node button, String eventKey)
	{
		
		button.FindChild("LabelInput").Set("text", eventKey.TrimSuffix(" (Physical)"));
	}


	private void _on_exit_button_button_down()
	{
		GetParent().GetNode<Control>("KeymapMenu/InputSettings").Visible = false;
	}
	
}
