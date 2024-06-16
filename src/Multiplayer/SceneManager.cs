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
	[Export] private PackedScene _bossScene;

	private static PackedScene[] _levels =
	{
		(PackedScene)ResourceLoader.Load("res://src/Map/Map1.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map2.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map3.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map4.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map5.tscn")
	};
	
	
	public int CouldownSumon = 0;
	private int _cyclespawn = 0;
	
	private int _enemyCount = 0; // counts the number of enemies that have been spawned in the game.

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
				item.Player = currentPlayer;
			}
			else if (item.Role == 2)
			{
				currentPlayer = _player2Scene.Instantiate<Player>();
				currentPlayer.Gravity = -currentPlayer.Gravity;
				currentPlayer.Reversed = true;
				item.Player = currentPlayer;
			}
			else
			{
				throw new ArgumentException("Role should be 1 or 2");
			}

			currentPlayer.Name = item.Id.ToString();
			AddChild(currentPlayer);
			index++;
		}


		Array<Node> bossNodes = GetTree().GetNodesInGroup("Boss");
		foreach (Node2D VARIABLE in bossNodes)
		{
			Boss boss;
			boss = _bossScene.Instantiate<Boss>();
			boss.tier = 2;
			AddChild(boss);
			boss.GlobalPosition = VARIABLE.GlobalPosition;
		}
		
		Array<Node> eNodes = GetTree().GetNodesInGroup("EnemySpawnPoints");
		foreach (Node2D VARIABLE in eNodes)
		{
			Enemy enemy;
			if (int.Parse(VARIABLE.Name) % 2 == 0)
			{
				enemy = _enemy1ReversedScene.Instantiate<Enemy>();
				enemy.Reversed = true;
			}
			else
			{
				enemy = _enemy1Scene.Instantiate<Enemy>();
				enemy.Reversed = false;
			}
			AddChild(enemy);
			enemy.GlobalPosition = VARIABLE.GlobalPosition;
		}
	}
	


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				Rpc("_pauseGame");
				
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void _pauseGame()
	{
		GD.Print("Execution of the function");
		GetNode<Control>("PauseMenu").Visible = !GetNode<Control>("PauseMenu").Visible;
	}

	private void _on_disconnect_button_pressed()
	{
		GetTree().Quit();
	}

	public void NextLevel(int i)
	{
		GetTree().ChangeSceneToPacked(_levels[i + 1]);
	}
	
}
