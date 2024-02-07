using System;
using Entity.players;
using Godot;

namespace OutofTheHole.multiplayer;

public partial class SceneManager : Node2D
{
	[Export] private PackedScene player1Scene;
	[Export] private PackedScene player2Scene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var index = 0;
		foreach (var item in GameManager.Players)
			if (item.Role == 1)
			{
				var currentPlayer = player1Scene.Instantiate<Player>();
				currentPlayer.Name = item.Id.ToString();
				currentPlayer.reversed = false;
				AddChild(currentPlayer);
				foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("SpawnPoints"))
					if (int.Parse(spawnPoint.Name) == index)
						currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
				index++;
			}
			else if (item.Role == 2)
			{
				var currentPlayer = player2Scene.Instantiate<Player>();
				currentPlayer.Name = item.Id.ToString();
				currentPlayer.Gravity = -currentPlayer.Gravity;
				currentPlayer.reversed = true;
				AddChild(currentPlayer);
				foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("SpawnPoints"))
					if (int.Parse(spawnPoint.Name) == index)
						currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
				index++;
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
