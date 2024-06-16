using Godot;
using System;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class EndLvl : Node2D
{
	
	public bool can;
	public override void _Ready()
	{
		can = true;
		if (int.Parse(Name) % 2 == 0)
		{
			GetNode<Sprite2D>("Sprite").Visible = false;
		}
	}
	
	private void _on_area_2d_body_entered(Node2D body) 
	{
		if (body is Player)
		{
				GameManager.Players[1].Player.GlobalPosition = body.GlobalPosition;
				GameManager.Players[0].Player.GlobalPosition = body.GlobalPosition;
			
				GD.Print(Owner.GetPath());
				GetTree().Root.AddChild(((PackedScene)ResourceLoader.Load("res://src/Map/Map2.tscn")).Instantiate());
				GD.Print($"{GetTree().Root.GetChild(1).GetPath()}");
				//Owner = GetTree().Root.GetChild(0);
				//GetTree().ChangeSceneToPacked();
				//((PackedScene)ResourceLoader.Load("res://src/Map/Map1.tscn")).Instantiate();
			
				Owner.Free();
		}
	}
}


