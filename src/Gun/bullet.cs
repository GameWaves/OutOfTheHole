using Godot;
using System;

public partial class bullet : RigidBody2D
{
	[Export] private int longetivity;
	public override void _Ready()
	{
		longetivity = 100;
	}

	public void _Process(int delta)
	{
		if (longetivity == 0)
		{
			QueueFree();
		}
		longetivity -= delta;
	}
}
