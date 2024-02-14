using System;
using Godot;
using OutofTheHole.Gun;

namespace OutOfTheHole.Bullet;

public abstract partial class Bullets : RigidBody2D
{
	
	protected AnimatedSprite2D bulletsprite;

	//speed of bullet
	public float Speed { get; set; }
	
	//direction of bullet
	protected Vector2 direction
	{
		get => new Vector2(1, 0).Rotated(Rotation);
		
	}
	
	//lifspan of bullet
	public int longetivity { get; set; }
	
	//damage of bullet (correspond to the number of hp he player/enemy will lose)
	public int Damage { get; set; }
	
	//gravity of bullet (and everything else)
	protected float gravity
	{
		get => ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}
	
	/// <summary>
	///     Updated 60 times per seconds, detects when the bullet needs to be destroyed.
	///		Write general code that will apply to all bullets
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(double delta)
	{
		LinearVelocity = Speed * direction;
		if (longetivity == 0)
		{
			QueueFree();
			if (IsQueuedForDeletion()) Free();
		}
		longetivity -= (int)Math.Ceiling(delta);
		
	}
	/// <summary>
	///		function to call if bullet hit player
	/// </summary>
	public virtual void OnHit(){}
	
}
