using Godot;
using System;

public partial class InputSettings : Control
{
	[Export] private PackedScene _inputButtonScene;
	private VBoxContainer _actionList;
	private bool _isRemapping = false;
	private object _actionToRemap = null;

	private object remappingButton = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_actionList = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/ActionList");
		CreateActionList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CreateActionList()
	{
		InputMap.LoadFromProjectSettings();
		foreach (var item in _actionList.GetChildren())
		{
			item.QueueFree();
		}

		foreach (var action in InputMap.GetActions())
		{
			GD.Print("Action triggered:" + action);
			Node button = _inputButtonScene.Instantiate();
			Node actionLabel = button.FindChild("LabelAction");
			Node inputLabel = button.FindChild("LabelInput");

			actionLabel.Set("text", action);

			var events = InputMap.ActionGetEvents(action);
			if (events.Count > 0)
			{
				inputLabel.Set("text", events[0].AsText());
			}
			else
			{
				inputLabel.Set("text", "Not set");
			}
			
			_actionList.AddChild(button);
		}
	}
}
