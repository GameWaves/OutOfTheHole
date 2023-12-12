using Godot;
using System;

public partial class Enemy1 : CharacterBody2D
{
	private Vector2 velocity = Vector2.Zero; // Enemy velocity
	private Random random = new Random(); // Random number generator

	// Called every frame
	public override void _Process(float delta)
	{
		// Check if the enemy should change direction
		if (random.Next(100) < 5) // 5% chance to change direction every frame (adjust as needed)
		{
			// Choose a new random direction
			velocity = new Vector2(random.Next(-1, 2), random.Next(-1, 2)).Normalized() * 100;
		}

		// Apply velocity to the enemy
		KinematicCollision2D collision = MoveAndCollide(velocity * delta);

		// If the enemy collides with a wall, choose a new random direction
		if (collision != null)
		{
			velocity = new Vector2(random.Next(-1, 2), random.Next(-1, 2)).Normalized() * 100;
		}
	}
} 
