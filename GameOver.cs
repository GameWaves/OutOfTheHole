using Godot;
using System;

public partial class GameOver : Node2D
{
	private void _on_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://Map.tscn");
	}
	
	private void _on_button_2_pressed()
	{
		GetTree().Quit();
	}
}






