using Godot;
using System;
using System.Collections.Generic;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class MapManager : Node2D
{

	[Export] public PackedScene _startMap;
	public Node _currentMap;
	public int _currentIndex = 0;
	

	private List<PackedScene> _levels = new List<PackedScene>()
	{
		(PackedScene)ResourceLoader.Load("res://src/Map/Map1.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map2.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map3.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map4.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Map/Map5.tscn")
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node startMap = _startMap.Instantiate();
		AddChild(startMap);
		_currentMap = startMap;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _nextScene(Node2D body)
	{
		
		body.GlobalPosition = new Vector2(0, 0);	
		_currentIndex += 1;
		Node ToAdd = _levels[_currentIndex].Instantiate();
		GetTree().Root.GetTree().ChangeSceneToPacked(_levels[_currentIndex]);
	
		Node Dest = GetTree().Root.GetNode(ToAdd.Name.ToString());

		Node Origin = GetTree().Root.GetNode("Map");

		
		
		foreach (var test in Origin.GetChildren())
		{
			GD.Print(test);
			test.Reparent(Dest);
		}
		
		
		
			
		GD.Print("Changed for ", _levels[_currentIndex].ResourcePath.ToString());
		if (_currentIndex <= 6)
		{
			

			/*GD.Print(GetParent().Name);
			Node ToFree = GetParent();

			Node Master = ToFree.GetParent();

			PackedScene ToAdd = _levels[_currentIndex];

			GetTree().Root.GetNode("MapMaster").AddChild(ToAdd.Instantiate());




			GD.Print(_levels.Count);*/


		}









	}
}
