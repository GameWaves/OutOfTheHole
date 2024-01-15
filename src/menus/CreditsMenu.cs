using Godot;
using System;

public partial class CreditsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_exit_button_button_down()
	{
		GetTree().ChangeSceneToFile("res://src/menus/MainMenu.tscn");
	}
}
