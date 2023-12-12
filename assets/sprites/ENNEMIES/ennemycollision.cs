using Godot;
using System;

public partial class Enemycollision: CharacterBody2D
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
		MoveAndCollide(velocity * delta);
	}

	// Called when entering the collision shape's area
	private void _on_Enemy_area_entered(Area2D area)
	{
		GD.Print("Collision with: " + area);
		// Handle the collision as needed
	}

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		// Connect the signal to the method
		Connect("area_entered", this, "_on_Enemy_area_entered");
	}
}
