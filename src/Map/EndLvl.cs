using Godot;
using System;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class EndLvl : Node2D
{
	[Export] private PackedScene Scene;
	public bool can;
	public override void _Ready()
	{
		can = true;
		if (int.Parse(Name) % 2 == 0)
		{
			GetNode<Sprite2D>("Sprite").Visible = false;
		}
		else
		{
			GetNode<Sprite2D>("SpriteReversed").Visible = false;
		}
	}
	
	private void _on_area_2d_body_entered(Node2D body) 
	{
		if (body is Player)
		{
			if (can)
			{
				can = false;
				SceneManager.lvl += 1;
			}
		}
		GetParent<Node2D>().Visible = false; 
		GetParent<Node2D>().Position = new Vector2(-10000000,10000000);
		GetTree().ChangeSceneToFile($"res://src/Map/Map{SceneManager.lvl}.tscn");
	}
	
}


