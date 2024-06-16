using Godot;
using System;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class EndLvl : Node2D
{
	
	public bool reached;
	public override void _Ready()
	{
		reached = false;
		if (int.Parse(Name) % 2 == 0)
		{
			GetNode<Sprite2D>("Sprite").Visible = false;
		}
	}
	
	private void _on_area_2d_body_entered(Node2D body) 
	{
		if (!reached && body == GameManager.Players[0].Player)
		{
			string level = (Owner.GetPath()).ToString().Split('.')[0];
			int lvl = int.Parse((level[^1]).ToString());
			reached = true;
			GD.Print(Owner.GetPath());
			GetTree().Root.AddChild(((PackedScene)ResourceLoader.Load($"res://src/Map/Map{lvl+1}.tscn")).Instantiate());
			GD.Print($"{GetTree().Root.GetChild(1).GetPath()}");
			//Owner = GetTree().Root.GetChild(0);
			//GetTree().ChangeSceneToPacked();
			//((PackedScene)ResourceLoader.Load("res://src/Map/Map1.tscn")).Instantiate();
			Owner.Free();
		}
		else
		{
			(GameManager.Players[1].Player.GlobalPosition, GameManager.Players[0].Player.GlobalPosition) =
				(GameManager.Players[0].Player.GlobalPosition, GameManager.Players[1].Player.GlobalPosition);
		}
	}

}


