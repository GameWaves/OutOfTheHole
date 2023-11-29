using Godot;
using OutofTheHole.players;

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
				var currentPlayer = player1Scene.Instantiate<mvplayer>();
				currentPlayer.Name = item.Id.ToString();
				AddChild(currentPlayer);
				foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("SpawnPoints"))
					if (int.Parse(spawnPoint.Name) == index)
						currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
				index++;
			}
			else if (item.Role == 2)
			{
				var currentPlayer = player2Scene.Instantiate<mvplayer2>();
				currentPlayer.Name = item.Id.ToString();
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
