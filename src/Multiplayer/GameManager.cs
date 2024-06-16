using System.Collections.Generic;
using Godot;

namespace OutofTheHole.Multiplayer;


public partial class GameManager : Node
{
	public static List<PlayerInfo> Players = new();
	public PackedScene[] _levels = new PackedScene[]
	{
		(PackedScene)ResourceLoader.Load("res://src/Maps/Map1.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Maps/Map2.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Maps/Map3.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Maps/Map4.tscn"),
		(PackedScene)ResourceLoader.Load("res://src/Maps/Map5.tscn")
	};

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
