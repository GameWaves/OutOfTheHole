using System.Collections.Generic;
using Godot;

namespace OutofTheHole.Multiplayer;


public partial class GameManager : Node
{
	public static List<PlayerInfo> Players = new();

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
