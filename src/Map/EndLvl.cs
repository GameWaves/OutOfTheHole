using Godot;
using System;
using OutofTheHole.Multiplayer;

public partial class EndLvl : Node2D
{
	
	public bool can;
	public override void _Ready()
	{
		can = true;

	}
	
	private void _on_area_2d_body_entered(Node2D body) 
	{
		if (can)
		{
			can = false;
			SceneManager.lvl += 1;
		}
		GetTree().ChangeSceneToFile($"res://src/Map/Map{SceneManager.lvl}.tscn");
	
	}
	
}



