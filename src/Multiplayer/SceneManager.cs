using System;
using Godot;
using OutOfTheHole.Entity.Enemies;
using OutofTheHole.Entity.Players;
using Enemy = OutOfTheHole.Entity.Enemies.Enemy;


namespace OutofTheHole.Multiplayer;

public partial class SceneManager : Node2D
{
	[Export] private PackedScene _player1Scene;
	[Export] private PackedScene _player2Scene;
	[Export] private PackedScene _enemyScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var index = 0;
		foreach (var item in GameManager.Players)
		{
			Player currentPlayer;
			if (item.Role == 1)
			{
				currentPlayer = _player1Scene.Instantiate<Player>();
				currentPlayer.Name = item.Id.ToString();
				currentPlayer.Reversed = false;
			}
			else if (item.Role == 2)
			{
				currentPlayer = _player2Scene.Instantiate<Player>();
				currentPlayer.Gravity = -currentPlayer.Gravity;
				currentPlayer.Reversed = true;
			}
			else
			{
				throw new ArgumentException("Role should be 1 or 2");
			}

			currentPlayer.Name = item.Id.ToString();
			AddChild(currentPlayer);
			InitEnemy();
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

	public void InitEnemy()
	{
		var ennemyScene = ResourceLoader.Load("res://src/Enemies/Enemy1.tscn") as PackedScene;
		var ennemy = ennemyScene.Instantiate<Enemy>();
		SceneTree sceneRoot = GetTree();
		sceneRoot.Root.AddChild(ennemy);
	}
}
