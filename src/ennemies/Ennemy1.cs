using Godot;
using System;

public partial class Ennemy1 : CharacterBody2D
{
	public float movespeed = 150.0f; // vitesse du personnage
	public float jumpVelocity = 400.0f; // vitesse vers le haut des sauts
	public float gravity = ProjectSettings.GetSetting("Physics/2d/default_gravity").AsSingle();
	public const int maxHp = 100; // set maxHp of ennemy
	public static bool alive; // to know if the ennemy is alive
	private int hp; // declare the hp variable
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set ennemy hp
		hp = maxHp;
		alive = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity=Velocity;
		// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
			if (!IsOnFloor())
				velocity.Y += gravity * (float)delta;

		
		Velocity=velocity;
	}
	public void ishurt(int n)
	{
		hp = hp - n;
		if (hp <= 0)
		{
			alive = false;
			// hide the player 
			// ennemy1.Visible = false; // to fix bcz it doesn't work
		}	
	}	
}
