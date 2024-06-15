using Godot;
using System;

public partial class Maps : Node2D
{
	public int i = 0;
	public override void _Ready()
	{
		GetNode("map1").Free();
		GetNode("map2").Free();
		GetNode("map3").Free();
		GetNode("map4").Free();
		GetNode("map5").Free();
		GetNode("map6").Free();
	}

	public void next()
	{
		GetTree().CurrentScene = GetNode($"map{i + 1}");
	}
	

}
