using Godot;
using System;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class EndLvl : Node2D
{

	[Export] private string _dest;
	private string _origin = "Map";
	
	public Player P1 = GameManager.Players[0].Player;
	public Player P2 = GameManager.Players[1].Player;
	
	public bool can;
	public override void _Ready()
	{
		can = true;

	}
	
	private void _on_area_2d_body_entered(Node2D body) 
	{
		//GD.Print($"BODY : {body.Name}");
		if (1==1)
		{
			Node Dest = new Node();
			Node Origin = new Node();
			can = false;
			SceneManager.lvl += 1;
			GD.Print("LVL: ", SceneManager.lvl);


			foreach (var child in GetTree().Root.GetChildren())
			{
				GD.Print("DEST: ", child);
				if (child.GetNode(_dest) != null)
					Dest = child.GetNode(_dest);
				if (child.GetNode(_origin) != null)
					Origin = child.GetNode(_origin);
			}

			foreach (var child in Origin.GetChildren())
			{
				if (child is Player)
					((Player)child).Teleport(0,0, false);
			}
			
			
			
			


		}
		

		//GetTree().ChangeSceneToFile($"res://src/Map/Map{SceneManager.lvl}.tscn");

	}
	
}



