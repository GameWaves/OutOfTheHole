using Godot;
using System;

public partial class CharacterBody2D : Godot.CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// Called every frame
	public override void _Process (float delta)
	{
		// Get player input
		velocity = new Vector2();

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.x += 1;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.x -= 1;
		}

		// Normalize the vector to ensure consistent movement speed in all directions
		velocity = velocity.Normalized() * speed;

		// Apply gravity
		velocity.y += gravity * delta;

		// Move the player
		velocity = MoveAndSlide(velocity, Vector2.Up);

		// Jumping
		if (IsOnFloor() && Input.IsActionPressed("ui_accept"))
		{
			velocity.y = jumpStrength;
		}
	}
}

