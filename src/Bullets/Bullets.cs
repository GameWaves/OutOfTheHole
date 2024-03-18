using System;
using Godot;
using Godot.Collections;
using OutofTheHole.Gun;
using Array = Godot.Collections.Array;

namespace OutOfTheHole.Bullet;

public abstract partial class Bullets : RigidBody2D
{
	
	protected AnimatedSprite2D BulletSprite;

	//speed of bullet
	public float Speed { get; protected set; }
	
	//direction of bullet
	protected Vector2 Direction
	{
		get => new Vector2(1, 0).Rotated(Rotation);
		
	}

	public OutofTheHole.Entity.Entity source;
	
	//lifspan of bullet
	public int Longetivity { get; set; }
	
	//damage of bullet (correspond to the number of hp he player/enemy will lose)
	public int Damage { get; set; }
	
	//gravity of bullet (and everything else)
	protected float Gravity
	{
		get => ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	/// <summary>
	/// Updated 60 times per seconds, detects when the bullet needs to be destroyed.
	///		Write general code that will apply to all bullets
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(double delta)
	{
		LinearVelocity = Speed * Direction;
		if (Longetivity == 0)
		{
			QueueFree();
			if (IsQueuedForDeletion()) Free();
		}
		Longetivity -= (int)Math.Ceiling(delta);

	}

	/// <summary>
	///		function to call if bullet hit player
	/// </summary>
	public abstract void OnHit(Node2D target);

	private void _on_area_2d_body_entered(Node2D body)
	{
		OnHit(body);
	}
}
