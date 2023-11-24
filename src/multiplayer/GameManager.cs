using Godot;
using System;
using OutofTheHole.multiplayer;
using OutofTheHole.players;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static List<PlayerInfo> Players = new List<PlayerInfo>();

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
