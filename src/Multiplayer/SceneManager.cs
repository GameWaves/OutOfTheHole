using System;
using Godot;
using Godot.Collections;
using OutOfTheHole.Entity.Enemies;
using OutofTheHole.Entity.Players;
using Array = Godot.Collections.Array;
using Enemy = OutOfTheHole.Entity.Enemies.Enemy;


namespace OutofTheHole.Multiplayer;

public partial class SceneManager : Node2D
{
	[Export] private PackedScene _player1Scene;
	[Export] private PackedScene _player2Scene;
	[Export] private PackedScene _enemy1Scene;
	[Export] private PackedScene _enemy1ReversedScene;

	public int CouldownSumon = 0;
	private int cyclespawn = 0;

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
			foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("SpawnPoints"))
				if (int.Parse(spawnPoint.Name) == index)
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
			index++;
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		//temporary feature : summon ennemy
		CouldownSumon -= 1;
		if (CouldownSumon < 0)
		{
			InitEnemy();
			CouldownSumon = 500;
		}
	}

	public void InitEnemy()
	{
		
		Array<Node> ES = GetTree().GetNodesInGroup("Enemy_1_spawn");
		
		if (cyclespawn%3 == 0)
		{
			Enemy enemy1_Rev;
			enemy1_Rev = _enemy1ReversedScene.Instantiate<Enemy>();
			enemy1_Rev.Reversed = true;
			AddChild(enemy1_Rev);
			enemy1_Rev.GlobalPosition = ((Node2D)ES[cyclespawn%2]).GlobalPosition;

		}
		else
		{
			Enemy enemy1;
			enemy1 = _enemy1Scene.Instantiate<Enemy>();
			enemy1.Reversed = false;
			AddChild(enemy1);
			enemy1.GlobalPosition = ((Node2D)ES[cyclespawn%2]).GlobalPosition;
		}

		cyclespawn = cyclespawn + 1;
		
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				GetNode<Control>("PauseMenu").Visible = !GetNode<Control>("PauseMenu").Visible;
	}

	private void _on_disconnect_button_pressed()
	{
		GetTree().Quit();
	}
}
